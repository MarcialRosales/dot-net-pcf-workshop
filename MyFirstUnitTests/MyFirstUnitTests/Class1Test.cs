using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

using Xunit;

// XUnit features:
//- ** Simple to use**: It just needs one attribute `[Fact]`.
//- ** Single object instance per test method**: It allows complete isolation of test methods that allow developers to independently run tests in any order.
// - **No Support for ExpectedException attribute**: The best way is that the developers handle these expected exceptions within the test method itself.

namespace MyFirstUnitTests
{
    // Trick: Structure Scenearios using "test class as context" pattern.
    // AddNumbers and SubstractNumbers are scenarios with their tests and their own setup/tearDown
    public class Class1Test
    {

        // Trick: We want to avoid "Class Fixtures" because test should be independent from each other
        // MAybe for complex end-to-end tests we can share the context, e.g. the session id.
        // When using a class fixture, xUnit.net will ensure that the fixture instance will be created
        // before any of the tests have run, and once all the tests have finished, it will clean up the fixture object by calling Dispose, if present.
        // 1. create ComplexFixture class
        // 2. extend our test class with IClassFixture<OurTestFixture>
        // 3. Add constructor which takes IClassFixture<OurTestFixture>
        public class AddNumbers : IClassFixture<ComplexFixture>
        {
            Class1 class1;
            ITestOutputHelper output;

            [Fact]
            public void SameNumbers()
            {
                output.WriteLine("Same numbers");
                Assert.Equal(4, class1.Add(2, 2));
            }

            // Trick: we want clean test context between tests but we want to share the same setup using Constructor and Dispose
            // Trick: we want to capture the output of test (https://xunit.github.io/docs/capturing-output.html)
            public AddNumbers(ITestOutputHelper output, ComplexFixture fixture)
            {
                this.output = output;
                output.WriteLine($"fixture: {fixture}");
                class1 = new Class1();
            }
        }

        public class SubstractNumbers
        {
            Class1 class1;

            public SubstractNumbers()
            {
                class1 = new Class1();
            }

            [Fact]
            public void BiggerFromSmallerShouldProduceNegativeNumber()
            {
                Assert.True(class1.Substract(2, 5) < 0);
            }

        }
    }
    public class ComplexFixture : IDisposable
    {

        public ComplexFixture()
        {
            // ... initialize complex set up like login, etc. ...
        }

        public void Dispose()
        {
            // ... clean up ...
        }

        // Provide getter
        // public SomeClass someClass { get; private set; }
    }
}
