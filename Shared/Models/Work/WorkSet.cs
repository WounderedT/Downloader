using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Models.Work
{
    public class WorkSet : ICollection<UnitOfWork>
    {
        private readonly Dictionary<String, UnitOfWork> _unitsOfWork;

        public static IEqualityComparer<WorkSet> EqualityComparer { get; } = new WorkSetEqualityComparer();
        public String Domain { get; }
        public String? FolderName { get; }
        public HttpClient? HttpClient { get; }
        public Boolean IsSuccess { get; internal set; }
        public Guid Id { get; } = Guid.NewGuid();

        public WorkSet(String baseUrl) : this(baseUrl, null, null)
        {
        }

        public WorkSet(String baseUrl, String folderName) : this(baseUrl, folderName, null)
        {
        }

        public WorkSet(String baseUrl, HttpClient httpClient) : this(baseUrl, null, httpClient)
        {
        }

        public WorkSet(String baseUrl, String? folderName, HttpClient? httpClient)
        {
            if (String.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            Domain = new Uri(baseUrl).Host;
            FolderName = folderName;
            HttpClient = httpClient;
            _unitsOfWork = new Dictionary<String, UnitOfWork>();
            IsReadOnly = false;
        }

        #region Implementation of IEnumerable

        /// <inheritdoc />
        public IEnumerator<UnitOfWork> GetEnumerator()
        {
            return _unitsOfWork.Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<ProcessResultEntity>

        /// <inheritdoc />
        public void Add(UnitOfWork item)
        {
            if (item == null)
            {
                return;
            }

            AddInternally(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _unitsOfWork.Clear();
        }

        /// <inheritdoc />
        public Boolean Contains(UnitOfWork item)
        {
            if (item == null)
            {
                return false;
            }

            return _unitsOfWork.TryGetValue(item.Filename, out UnitOfWork? value) && value.Url.Equals(item.Url);
        }

        /// <inheritdoc />
        public void CopyTo(UnitOfWork[] array, Int32 arrayIndex)
        {
            if (_unitsOfWork.Count > array.Length - arrayIndex)
            {
                var newArray = new UnitOfWork[array.Length + (_unitsOfWork.Count - (array.Length - arrayIndex))];
                array.CopyTo(newArray, 0);
                array = newArray;
            }

            Dictionary<String, UnitOfWork>.ValueCollection.Enumerator enumerator = _unitsOfWork.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array[arrayIndex++] = enumerator.Current;
            }

            enumerator.Dispose();
        }

        /// <inheritdoc />
        public Boolean Remove(UnitOfWork item)
        {
            return Contains(item) && _unitsOfWork.Remove(item.Filename);
        }

        /// <inheritdoc />
        public Int32 Count => _unitsOfWork.Count;

        /// <inheritdoc />
        public Boolean IsReadOnly { get; }

        #endregion

        public Boolean TryAdd(UnitOfWork item)
        {
            return item != null && AddInternally(item);
        }

        /// <summary>
        /// Rechecks download state and updates <see cref="IsSuccess"/> property.
        /// Warning: performance unfriendly method.
        /// </summary>
        public void RecheckState()
        {
            if(_unitsOfWork == null)
            {
                return;
            }
            IsSuccess = !_unitsOfWork.Any(u => !u.Value.IsDownloaded);
        }

        private Boolean AddInternally(UnitOfWork item)
        {
            while (true)
            {
                if (_unitsOfWork.TryGetValue(item.Filename, out UnitOfWork? existingEntity))
                {
                    if (existingEntity.Url.Equals(item.Url))
                    {
                        //Ignore identical link and notify the user
                        return false;
                    }

                    item.IndexFilename();
                    continue;
                }

                _unitsOfWork.Add(item.Filename, item);
                return true;
            }
        }

        public override Boolean Equals(Object? obj)
        {
            return obj is WorkSet ? Equals(obj as WorkSet) : false;
        }

        public Boolean Equals(WorkSet? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if(_unitsOfWork.Count != obj.Count)
            {
                return false;
            }
            foreach(var unitOfWork in _unitsOfWork)
            {
                if (!obj.Contains(unitOfWork.Value)) 
                { 
                    return false; 
                }
            }
            return true;
        }

        public override Int32 GetHashCode()
        {
            return Domain.GetHashCode();
        }
    }

    public class WorkSetEqualityComparer : IEqualityComparer<WorkSet>
    {
        public Boolean Equals(WorkSet? x, WorkSet? y)
        {
            if (x == null)
            {
                return y == null;
            }
            if (y == null)
            {
                return false;
            }
            return x.Equals(y);
        }

        public Int32 GetHashCode([DisallowNull] WorkSet obj)
        {
            return obj.GetHashCode();
        }
    }
}
