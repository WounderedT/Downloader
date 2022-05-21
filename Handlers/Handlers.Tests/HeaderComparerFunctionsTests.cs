using Handlers.Strategy;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Tests
{
    [TestClass]
    public class HeaderComparerFunctionsTests
    {
        private HttpContentHeaders GetHttpContentHeaders()
        {
            var response = new HttpResponseMessage();
            return response.Content.Headers;
        }

        #region Allow Header

        [TestMethod]
        public void Allow_LessThan_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.LessThan, null));
        }

        [TestMethod]
        public void Allow_LessThanOrEqualTo_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.LessThanOrEqualTo, null));
        }

        [TestMethod]
        public void Allow_GreaterThan_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.GreaterThan, null));
        }

        [TestMethod]
        public void Allow_GreaterThanOrEqualTo_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.GreaterThanOrEqualTo, null));
        }

        [TestMethod]
        public void Allow_EqualsTo_EmptyList_Success()
        {
            var comparisonValue = new List<String>();
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.AllowHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void Allow_EqualsTo_Null_Success()
        {
            ICollection<String>? comparisonValue = null;
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.AllowHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void Allow_EqualsTo_EqualLists_Success()
        {
            var comparisonValue = new List<String>() { "test1", "test2" };
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.AllowHeader, new List<String>() { "test1", "test2" });

            var function = HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void Allow_EqualsTo_DifferentCount_Success()
        {
            var comparisonValue = new List<String>() { "test1", "test2" };
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.AllowHeader, new List<String>() { "test1" });

            var function = HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void Allow_EqualsTo_DifferentLists_Success()
        {
            var comparisonValue = new List<String>() { "test1", "test2" };
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.AllowHeader, new List<String>() { "test2", "test1" });

            var function = HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void Allow_EqualsTo_SameObject_Success()
        {
            var comparisonValue = new List<String>() { "test1" };
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.AllowHeader, comparisonValue);

            var function = HeaderComparerFunctions.GetComparisonFunc_Allow(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        #endregion

        #region ContentEncoding Header

        [TestMethod]
        public void ContentEncoding_LessThan_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.LessThan, null));
        }

        [TestMethod]
        public void ContentEncoding_LessThanOrEqualTo_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.LessThanOrEqualTo, null));
        }

        [TestMethod]
        public void ContentEncoding_GreaterThan_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.GreaterThan, null));
        }

        [TestMethod]
        public void ContentEncoding_GreaterThanOrEqualTo_Exception()
        {
            Assert.ThrowsException<ArgumentException>(() => HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.GreaterThanOrEqualTo, null));
        }

        [TestMethod]
        public void ContentEncoding_EqualsTo_EmptyList_Success()
        {
            var comparisonValue = new List<String>();
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentEncoding_EqualsTo_Null_Success()
        {
            ICollection<String>? comparisonValue = null;
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentEncoding_EqualsTo_DifferentCount_Success()
        {
            var comparisonValue = new List<String>() { "test1", "test2" };
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>() { "test1" });

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentEncoding_EqualsTo_DifferentLists_Success()
        {
            var comparisonValue = new List<String>() { "test1", "test2" };
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>() { "test2", "test1" });

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        #endregion

        #region ContentLength Header

        [TestMethod]
        public void ContentLength_EqualsTo_Zero_Success()
        {
            var comparisonValue = 0;
            var httpContentHeaders = GetHttpContentHeaders();

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentLength(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentLength_EqualsTo_Null_Success()
        {
            Int64? comparisonValue = null;
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentLength(Models.ComparisonEnum.EqualsTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentLength_LessThanOrEqualTo_Zero_Success()
        {
            var comparisonValue = 0;
            var httpContentHeaders = GetHttpContentHeaders();

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentLength(Models.ComparisonEnum.LessThanOrEqualTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentLength_LessThanOrEqualTo_Null_Success()
        {
            Int64? comparisonValue = null;
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentLength(Models.ComparisonEnum.LessThanOrEqualTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentLength_GreaterThanOrEqualTo_Zero_Success()
        {
            var comparisonValue = 0;
            var httpContentHeaders = GetHttpContentHeaders();

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentLength(Models.ComparisonEnum.GreaterThanOrEqualTo, comparisonValue);
            Assert.IsTrue(function.Invoke(httpContentHeaders));
        }

        [TestMethod]
        public void ContentLength_GreaterThanOrEqualTo_Null_Success()
        {
            Int64? comparisonValue = null;
            var httpContentHeaders = GetHttpContentHeaders();
            httpContentHeaders.Add(Constants.ContentEncodingHeader, new List<String>());

            var function = HeaderComparerFunctions.GetComparisonFunc_ContentLength(Models.ComparisonEnum.GreaterThanOrEqualTo, comparisonValue);
            Assert.IsFalse(function.Invoke(httpContentHeaders));
        }

        #endregion
    }
}
