﻿using System;
using System.Collections.Generic;
using System.Linq;
using DapperExtensions.Test.Data;
using NUnit.Framework;

namespace DapperExtensions.Test.IntegrationTests
{
    [TestFixture]
    public class CrudFixture
    {
        public class InsertMethod : IntegrationBaseFixture
        {
            [Test]
            public void AddsEntityToDatabase_ReturnsKey()
            {
                RunTest(c =>
                            {
                                Person p = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
                                int id = c.Insert(p);
                                Assert.AreEqual(1, id);
                                Assert.AreEqual(1, p.Id);
                            });
            }

            [Test]
            public void AddsEntityToDatabase_ReturnsCompositeKey()
            {
                RunTest(c =>
                            {
                                Multikey m = new Multikey {Key2 = "key", Value = "foo"};
                                var key = c.Insert(m);
                                Assert.AreEqual(1, key.Key1);
                                Assert.AreEqual("key", key.Key2);
                            });
            }

            [Test]
            public void AddsEntityToDatabase_ReturnsGeneratedPrimaryKey()
            {
                RunTest(c =>
                            {
                                Animal a1 = new Animal {Name = "Foo"};
                                c.Insert(a1);

                                var a2 = c.Get<Animal>(a1.Id);
                                Assert.AreNotEqual(Guid.Empty, a2.Id);
                                Assert.AreEqual(a1.Id, a2.Id);
                            });
            }

            [Test]
            public void AddsMultipleEntitiesToDatabase()
            {
                RunTest(c =>
                            {
                                Animal a1 = new Animal {Name = "Foo"};
                                Animal a2 = new Animal {Name = "Bar"};
                                Animal a3 = new Animal {Name = "Baz"};

                                c.Insert<Animal>(new[] {a1, a2, a3});

                                var animals = c.GetList<Animal>().ToList();
                                Assert.AreEqual(3, animals.Count);
                            });
            }
        }

        public class GetMethod : IntegrationBaseFixture
        {
            [Test]
            public void UsingKey_ReturnsEntity()
            {
                RunTest(c =>
                            {
                                Person p1 = new Person
                                                {
                                                    Active = true,
                                                    FirstName = "Foo",
                                                    LastName = "Bar",
                                                    DateCreated = DateTime.UtcNow
                                                };
                                int id = c.Insert(p1);

                                Person p2 = c.Get<Person>(id);
                                Assert.AreEqual(id, p2.Id);
                                Assert.AreEqual("Foo", p2.FirstName);
                                Assert.AreEqual("Bar", p2.LastName);
                            });
            }

            [Test]
            public void UsingCompositeKey_ReturnsEntity()
            {
                RunTest(c =>
                            {
                                Multikey m1 = new Multikey {Key2 = "key", Value = "bar"};
                                var key = c.Insert(m1);

                                Multikey m2 = c.Get<Multikey>(new {key.Key1, key.Key2});
                                Assert.AreEqual(1, m2.Key1);
                                Assert.AreEqual("key", m2.Key2);
                                Assert.AreEqual("bar", m2.Value);
                            });
            }
        }

        public class DeleteMethod : IntegrationBaseFixture
        {
            [Test]
            public void UsingKey_DeletesFromDatabase()
            {
                RunTest(c =>
                            {
                                Person p1 = new Person
                                                {
                                                    Active = true,
                                                    FirstName = "Foo",
                                                    LastName = "Bar",
                                                    DateCreated = DateTime.UtcNow
                                                };
                                int id = c.Insert(p1);

                                Person p2 = c.Get<Person>(id);
                                c.Delete(p2);
                                Assert.IsNull(c.Get<Person>(id));
                            });
            }

            [Test]
            public void UsingCompositeKey_DeletesFromDatabase()
            {
                RunTest(c =>
                            {
                                Multikey m1 = new Multikey {Key2 = "key", Value = "bar"};
                                var key = c.Insert(m1);

                                Multikey m2 = c.Get<Multikey>(new {key.Key1, key.Key2});
                                c.Delete(m2);
                                Assert.IsNull(c.Get<Multikey>(new {key.Key1, key.Key2}));
                            });
            }

            [Test]
            public void UsingPredicate_DeletesRows()
            {
                RunTest(c =>
                            {
                                Person p1 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
                                Person p2 = new Person { Active = true, FirstName = "Foo", LastName = "Bar", DateCreated = DateTime.UtcNow };
                                Person p3 = new Person { Active = true, FirstName = "Foo", LastName = "Barz", DateCreated = DateTime.UtcNow };
                                c.Insert(p1);
                                c.Insert(p2);
                                c.Insert(p3);

                                var list = c.GetList<Person>();
                                Assert.AreEqual(3, list.Count());

                                IPredicate pred = Predicates.Field<Person>(p => p.LastName, Operator.Eq, "Bar");
                                var result = c.Delete<Person>(pred);
                                Assert.IsTrue(result);

                                list = c.GetList<Person>();
                                Assert.AreEqual(1, list.Count());
                            });
            }

        }

        public class UpdateMethod : IntegrationBaseFixture
        {
            [Test]
            public void UsingKey_UpdatesEntity()
            {
                RunTest(c =>
                            {
                                Person p1 = new Person
                                                {
                                                    Active = true,
                                                    FirstName = "Foo",
                                                    LastName = "Bar",
                                                    DateCreated = DateTime.UtcNow
                                                };
                                int id = c.Insert(p1);

                                var p2 = c.Get<Person>(id);
                                p2.FirstName = "Baz";
                                p2.Active = false;

                                c.Update(p2);

                                var p3 = c.Get<Person>(id);
                                Assert.AreEqual("Baz", p3.FirstName);
                                Assert.AreEqual("Bar", p3.LastName);
                                Assert.AreEqual(false, p3.Active);
                            });
            }

            [Test]
            public void UsingCompositeKey_UpdatesEntity()
            {
                RunTest(c =>
                            {
                                Multikey m1 = new Multikey {Key2 = "key", Value = "bar"};
                                var key = c.Insert(m1);

                                Multikey m2 = c.Get<Multikey>(new {key.Key1, key.Key2});
                                m2.Key2 = "key";
                                m2.Value = "barz";
                                c.Update(m2);

                                Multikey m3 = c.Get<Multikey>(new {Key1 = 1, Key2 = "key"});
                                Assert.AreEqual(1, m3.Key1);
                                Assert.AreEqual("key", m3.Key2);
                                Assert.AreEqual("barz", m3.Value);
                            });
            }
        }

        public class GetListMethod : IntegrationBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsAll()
            {
                RunTest(c =>
                            {
                                c.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
                                c.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
                                c.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
                                c.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

                                IEnumerable<Person> list = c.GetList<Person>();
                                Assert.AreEqual(4, list.Count());
                            });
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                RunTest(c =>
                            {
                                c.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow });
                                c.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow });
                                c.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow });
                                c.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow });

                                var predicate = Predicates.Field<Person>(f => f.Active, Operator.Eq, true);
                                IEnumerable<Person> list = c.GetList<Person>(predicate, null);
                                Assert.AreEqual(2, list.Count());
                                Assert.IsTrue(list.All(p => p.FirstName == "a" || p.FirstName == "c"));
                            });
                            }
        }

        public class GetPageMethod : IntegrationBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsMatching()
            {
                RunTest(c =>
                            {
                                var id1 = c.Insert(new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
                                var id2 = c.Insert(new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
                                var id3 = c.Insert(new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
                                var id4 = c.Insert(new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

                                IList<ISort> sort = new List<ISort>
                                    {
                                        Predicates.Sort<Person>(p => p.LastName),
                                        Predicates.Sort<Person>(p => p.FirstName)
                                    };

                                IEnumerable<Person> list = c.GetPage<Person>(null, sort, 0, 2);
                                Assert.AreEqual(2, list.Count());
                                Assert.AreEqual(id2, list.First().Id);
                                Assert.AreEqual(id1, list.Skip(1).First().Id);
                            });
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                RunTest(c =>
                            {
                                var id1 = c.Insert(new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
                                var id2 = c.Insert(new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
                                var id3 = c.Insert(new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
                                var id4 = c.Insert(new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

                                var predicate = Predicates.Field<Person>(f => f.Active, Operator.Eq, true);
                                IList<ISort> sort = new List<ISort>
                                    {
                                        Predicates.Sort<Person>(p => p.LastName),
                                        Predicates.Sort<Person>(p => p.FirstName)
                                    };

                                IEnumerable<Person> list = c.GetPage<Person>(predicate, sort, 0, 3);
                                Assert.AreEqual(2, list.Count());
                                Assert.IsTrue(list.All(p => p.FirstName == "Sigma" || p.FirstName == "Theta"));
                            });
            }

            [Test]
            public void NotFirstPage_Returns_NextResults()
            {
                RunTest(c =>
                            {
                                var id1 = c.Insert(new Person { Active = true, FirstName = "Sigma", LastName = "Alpha", DateCreated = DateTime.UtcNow });
                                var id2 = c.Insert(new Person { Active = false, FirstName = "Delta", LastName = "Alpha", DateCreated = DateTime.UtcNow });
                                var id3 = c.Insert(new Person { Active = true, FirstName = "Theta", LastName = "Gamma", DateCreated = DateTime.UtcNow });
                                var id4 = c.Insert(new Person { Active = false, FirstName = "Iota", LastName = "Beta", DateCreated = DateTime.UtcNow });

                                IList<ISort> sort = new List<ISort>
                                    {
                                        Predicates.Sort<Person>(p => p.LastName),
                                        Predicates.Sort<Person>(p => p.FirstName)
                                    };

                                IEnumerable<Person> list = c.GetPage<Person>(null, sort, 1, 2);
                                Assert.AreEqual(2, list.Count());
                                Assert.AreEqual(id4, list.First().Id);
                                Assert.AreEqual(id3, list.Skip(1).First().Id);
                            });
            }
        }

        public class CountMethod : IntegrationBaseFixture
        {
            [Test]
            public void UsingNullPredicate_Returns_Count()
            {
                RunTest(c =>
                            {
                                c.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
                                c.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
                                c.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
                                c.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

                                int count = c.Count<Person>(null);
                                Assert.AreEqual(4, count);
                            });
            }

            [Test]
            public void UsingPredicate_Returns_Count()
            {
                RunTest(c =>
                            {
                                c.Insert(new Person { Active = true, FirstName = "a", LastName = "a1", DateCreated = DateTime.UtcNow.AddDays(-10) });
                                c.Insert(new Person { Active = false, FirstName = "b", LastName = "b1", DateCreated = DateTime.UtcNow.AddDays(-10) });
                                c.Insert(new Person { Active = true, FirstName = "c", LastName = "c1", DateCreated = DateTime.UtcNow.AddDays(-3) });
                                c.Insert(new Person { Active = false, FirstName = "d", LastName = "d1", DateCreated = DateTime.UtcNow.AddDays(-1) });

                                var predicate = Predicates.Field<Person>(f => f.DateCreated, Operator.Lt, DateTime.UtcNow.AddDays(-5));
                                int count = c.Count<Person>(predicate);
                                Assert.AreEqual(2, count);
                            });
            }
        }
    }
}