using System;
using System.Linq;
using Neo4jClient;

namespace Neo4JinCSharp
{
    class Program
    {
        static GraphClient client;
        const string CONNECTION_STRING = "http://localhost:7474/db/data";

        static void Main(string[] args)
        {
            connect();

            createStudent(new Student { name = "John", tnumber = "t00654321" });
            createStudent(new Student { name = "Frank", tnumber = "t00123456" });

            createRelationship();

            var query = client.Cypher
                .Match("(s:Student)-[r]-(m:Student)")
                .Where((Student s) => s.tnumber == "t00654321")
                .Return((s, r, m) => new
                {
                    N = s.As<Student>(),
                    M = m.As<Student>(),
                    R = r.As<RelationshipInstance<object>>()
                });

            var results = query.Results;

            foreach (var item in results.ToList())
                Console.WriteLine("({0})-[:{1}]-({2})", item.N.name, item.R.TypeKey, item.M.name);
                Console.ReadLine();
        }

        static void createRelationship()
        {
            client.Cypher
                .Match("(student1:Student)", "(student2:Student)")
                .Where((Student student1) => student1.tnumber == "t00654321")
                .AndWhere((Student student2) => student2.tnumber == "t00123456")
                .Create("student1-[:FRIENDS_WITH]->student2")
                .ExecuteWithoutResults();
            Console.WriteLine("Relationship created...");
            Console.ReadLine();
        }

        static void createStudent(Student student) {
            client.Cypher
                .Create("(student:Student {newStudent})")
                .WithParam("newStudent", student)
                .ExecuteWithoutResults();
            Console.WriteLine("Student " + student.name + " created...");
            Console.ReadLine();
        }

        static void connect() {
            client = new GraphClient(new Uri(CONNECTION_STRING));
            client.Connect();
            Console.WriteLine("Connected to Neo4J " + client.ServerVersion);
            Console.ReadLine();
        }
    }
}
