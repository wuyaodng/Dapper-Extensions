﻿using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Sql;
using NUnit.Framework;

namespace DapperExtensions.Test.Sql
{
    public class SqlDialectBaseFixture
    {
        [TestFixture]
        public class QuoteTests
        {
            [Test]
            public void IsQuoted_WithQuotes_ReturnsTrue()
            {
                TestDialect dialect = GetDialect();
                Assert.IsTrue(dialect.IsQuoted("\"foo\""));
            }

            [Test]
            public void IsQuoted_WithOnlyStartQuotes_ReturnsFalse()
            {
                TestDialect dialect = GetDialect();
                Assert.IsFalse(dialect.IsQuoted("\"foo"));
            }

            [Test]
            public void IsQuoted_WithOnlyCloseQuotes_ReturnsFalse()
            {
                TestDialect dialect = GetDialect();
                Assert.IsFalse(dialect.IsQuoted("foo\""));
            }

            [Test]
            public void QuoteString_WithNoQuotes_AddsQuotes()
            {
                TestDialect dialect = GetDialect();
                Assert.AreEqual("\"foo\"", dialect.QuoteString("foo"));
            }

            [Test]
            public void QuoteString_WithStartQuote_AddsQuotes()
            {
                TestDialect dialect = GetDialect();
                Assert.AreEqual("\"\"foo\"", dialect.QuoteString("\"foo"));
            }

            [Test]
            public void QuoteString_WithCloseQuote_AddsQuotes()
            {
                TestDialect dialect = GetDialect();
                Assert.AreEqual("\"foo\"\"", dialect.QuoteString("foo\""));
            }

            [Test]
            public void QuoteString_WithBothQuote_DoesNotAddQuotes()
            {
                TestDialect dialect = GetDialect();
                Assert.AreEqual("\"foo\"", dialect.QuoteString("\"foo\""));
            }
        }

        [TestFixture]
        public class GetTableNameTests
        {
            [Test]
            public void NullTableName_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                Assert.Throws<ArgumentNullException>(() => dialect.GetTableName(null, null, null));
            }

            [Test]
            public void EmptyTableName_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                Assert.Throws<ArgumentNullException>(() => dialect.GetTableName(null, string.Empty, null));
            }

            [Test]
            public void TableNameOnly_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetTableName(null, "foo", null);
                Assert.AreEqual("\"foo\"", result);
            }

            [Test]
            public void SchemaAndTable_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetTableName("bar", "foo", null);
                Assert.AreEqual("\"bar\".\"foo\"", result);
            }

            [Test]
            public void AllParams_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetTableName("bar", "foo", "al");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }

            [Test]
            public void ContainsQuotes_DoesNotAddExtraQuotes()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetTableName("\"bar\"", "\"foo\"", "\"al\"");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }
        }

        [TestFixture]
        public class GetColumnNameTests
        {
            [Test]
            public void NullColumnName_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                Assert.Throws<ArgumentNullException>(() => dialect.GetColumnName(null, null, null));
            }

            [Test]
            public void EmptyColumnName_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                Assert.Throws<ArgumentNullException>(() => dialect.GetColumnName(null, string.Empty, null));
            }

            [Test]
            public void ColumnNameOnly_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetColumnName(null, "foo", null);
                Assert.AreEqual("\"foo\"", result);
            }

            [Test]
            public void PrefixColumnName_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetColumnName("bar", "foo", null);
                Assert.AreEqual("\"bar\".\"foo\"", result);
            }

            [Test]
            public void AllParams_ReturnsProperlyQuoted()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetColumnName("bar", "foo", "al");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }

            [Test]
            public void ContainsQuotes_DoesNotAddExtraQuotes()
            {
                TestDialect dialect = GetDialect();
                string result = dialect.GetColumnName("\"bar\"", "\"foo\"", "\"al\"");
                Assert.AreEqual("\"bar\".\"foo\" AS \"al\"", result);
            }
        }

        private static TestDialect GetDialect()
        {
            return new TestDialect();
        }

        private class TestDialect : SqlDialectBase
        {
            public override string GetIdentitySql(string tableName)
            {
                throw new NotImplementedException();
            }

            public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
            {
                throw new NotImplementedException();
            }
        }
    }
}