using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MyFirstUnitTests
{
    public interface ISomethingToReplace
    {
        int ReturnAValue(int v);
        void DoSomething();
        void AnotherBehaviour();
    }
    // System ULnder Test
    class SUT
    {
        private ISomethingToReplace something;

        public SUT(ISomethingToReplace something)
        {
            this.something = something;
        }

        public int GetValue(int val)
        {
            return something.ReturnAValue(val);
        }
        public void DoSomething()
        {
            something.DoSomething();
        }
        public void DoSomethingWithValue(int value)
        {
            something.ReturnAValue(value);
        }
        public void AnotherBehavior()
        {
            something.AnotherBehaviour();
        }
    } 
    public class TestDoubles
    {

        [Fact]
        public void Dummy()
        {
            var stub = new Mock<ISomethingToReplace>();
            var SUT = new SUT(stub.Object);
            
            SUT.AnotherBehavior();
        }

        [Fact]
        public void Stub()
        {
            var stub = new Mock<ISomethingToReplace>();
            var SUT = new SUT(stub.Object);
            
            stub.Setup(s => s.ReturnAValue(3)).Returns(3);

            SUT.GetValue(3);

            stub.VerifyAll();
        }

        [Fact]
        public void Spy()
        {
            var spy = new Mock<ISomethingToReplace>();
            var SUT = new SUT(spy.Object);

            spy.Setup(s => s.DoSomething());

            SUT.DoSomething();

            spy.VerifyAll();
        }

        [Fact]
        public void mock()
        {
            var mock = new Mock<ISomethingToReplace>();
            var SUT = new SUT(mock.Object);

            var sum = 0;

            mock.Setup(s => s.ReturnAValue(It.IsAny<int>())).Returns(3).Callback<int>((value) => sum+=value);

            SUT.DoSomethingWithValue(2);
            SUT.DoSomethingWithValue(3);

            sum.Should().Be(5);
            

        }


    }
}
