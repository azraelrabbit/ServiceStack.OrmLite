using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace ServiceStack.OrmLite.Tests.UseCase
{
	[TestFixture]
	public class SimpleUseCase : OrmLiteTestBase
	{
		public class User
		{
			public long Id { get; set; }

			[DataAnnotations.Index]
			public string Name { get; set; }

			public DateTime CreatedDate { get; set; }
		}

		[Test]
		public void Simple_CRUD_example()
		{
			using (IDbConnection db = OpenDbConnection())
			{
				db.DropAndCreateTable<User>();

				db.Insert(new User { Id = 1, Name = "A", CreatedDate = DateTime.Now });
				db.Insert(new User { Id = 2, Name = "B", CreatedDate = DateTime.Now });
				db.Insert(new User { Id = 3, Name = "B", CreatedDate = DateTime.Now });

				var rowsB = db.SelectFmt<User>("Name = {0}", "B");

				Assert.That(rowsB, Has.Count.EqualTo(2));

				var rowIds = rowsB.ConvertAll(x => x.Id);
				Assert.That(rowIds, Is.EquivalentTo(new List<long> { 2, 3 }));

				rowsB.ForEach(x => db.Delete(x));

				rowsB = db.SelectFmt<User>("Name = {0}", "B");
				Assert.That(rowsB, Has.Count.EqualTo(0));

				var rowsLeft = db.Select<User>();
				Assert.That(rowsLeft, Has.Count.EqualTo(1));

				Assert.That(rowsLeft[0].Name, Is.EqualTo("A"));
			}
		}

	}

}