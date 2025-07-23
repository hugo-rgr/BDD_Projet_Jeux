using System;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace BDD_Projet_Jeux_Tests.Steps
{
    [Binding]
    public sealed class SharedStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        public SharedStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Then(@"une erreur ""(.*)"" est levée")]
        public void ThenUneErreurEstLevee(string messageErreur)
        {
            var exception = _scenarioContext["exception"] as Exception;
            Assert.IsNotNull(exception, "Aucune exception n'a été capturée");
            Assert.IsTrue(exception.Message.Contains(messageErreur), 
                $"Le message d'erreur attendu '{messageErreur}' n'a pas été trouvé. Message actuel: '{exception.Message}'");
        }
    }
}