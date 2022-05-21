using System.Collections;

namespace Shared.Models.Work
{
    public class WorkRequest : ICollection<WorkSet>
    {
        private readonly HashSet<WorkSet> _work;
        
        public IEnumerable<String> Urls
        {
            get
            {
                if(_work == null || _work.Count == 0) 
                { 
                    return Enumerable.Empty<String>(); 
                }
                return _work.SelectMany(set => set.Select(unit => unit.Url.AbsoluteUri));
            }
        }

        public WorkRequest()
        {
            _work = new HashSet<WorkSet>(WorkSet.EqualityComparer);
            IsReadOnly = false;
        }

        #region Implementation of IEnumerable

        /// <inheritdoc />
        public IEnumerator<WorkSet> GetEnumerator()
        {
            return _work.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<WorkEntry>

        /// <inheritdoc />
        public void Add(WorkSet item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!AddInternally(item))
            {
                throw new ArgumentException($"A WorkSet element with the same ID already exists in the {typeof(WorkRequest).FullName}");
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            _work.Clear();
        }

        /// <inheritdoc />
        public Boolean Contains(WorkSet item)
        {
            if (item == null)
            {
                return false;
            }

            return _work.TryGetValue(item, out _);
        }

        /// <inheritdoc />
        public void CopyTo(WorkSet[] array, Int32 arrayIndex)
        {
            if (_work.Count > array.Length - arrayIndex)
            {
                var newArray = new WorkSet[array.Length + (_work.Count - (array.Length - arrayIndex))];
                array.CopyTo(newArray, 0);
                array = newArray;
            }

            HashSet<WorkSet>.Enumerator enumerator = _work.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array[arrayIndex++] = enumerator.Current;
            }

            enumerator.Dispose();
        }

        /// <inheritdoc />
        public Boolean Remove(WorkSet item)
        {
            return item != null && _work.Remove(item);
        }

        /// <inheritdoc />
        public Int32 Count => _work.Count;

        /// <inheritdoc />
        public Boolean IsReadOnly { get; }

        #endregion

        public Boolean TryAdd(WorkSet item)
        {
            if (item == null)
            {
                return false;
            }

            return AddInternally(item);
        }

        public void AddRange(IEnumerable<WorkSet> workSets)
        {
            if(workSets == null)
            {
                throw new ArgumentNullException(nameof(workSets));
            }
            foreach(var workSet in workSets)
            {
                _work.Add(workSet);
            }
        }

        public void Merge(WorkRequest workSetCollection)
        {
            if (workSetCollection == null)
            {
                throw new ArgumentNullException(nameof(workSetCollection));
            }
            if (workSetCollection.Count == 0)
            {
                return;
            }
            foreach (WorkSet workSet in workSetCollection)
            {
                AddInternally(workSet);
            }
        }

        private Boolean AddInternally(WorkSet item)
        {
            if (_work.Contains(item))
            {
                return false;
            }

            _work.Add(item);
            return true;
        }
    }
}
