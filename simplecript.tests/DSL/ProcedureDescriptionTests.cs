using NUnit.Framework;
using simplescript.DSL;
using simplescript.tests.Abstract;
using System.IO;

namespace simplescript.tests.DSL
{
    [TestFixture]
    public class ProcedureDescriptionTests
    {
        public void Acceptance()
        {
            var proc = ProcedureDescription<object>.
                Start().
                Then(new ProcedureStepBaseTestExposal<object>()).
                UseService(() => new FileStream("", FileMode.Open)).
                Then(new ProcedureStepBaseTestExposal<object>()).
                Then(new ProcedureStepBaseTestExposal<object>()).
                Finish();

            proc.Execute(new object());


        }
    }
}
