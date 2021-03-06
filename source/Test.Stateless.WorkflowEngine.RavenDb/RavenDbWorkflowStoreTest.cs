﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stateless.WorkflowEngine;
using Stateless.WorkflowEngine.Stores;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Extensions;
using Raven.Client.Embedded;
using System.IO;
using Raven.Client.Document;
using Stateless.WorkflowEngine.Models;
using Test.Stateless.WorkflowEngine.Workflows.Basic;
using Test.Stateless.WorkflowEngine.Workflows.Broken;
using Test.Stateless.WorkflowEngine.Workflows.Delayed;
using Test.Stateless.WorkflowEngine.Workflows.SimpleTwoState;
using Test.Stateless.WorkflowEngine.Stores;
using Stateless.WorkflowEngine.RavenDb;

namespace Test.Stateless.WorkflowEngine.RavenDb
{
    /// <summary>
    /// Test fixture for RavenDbWorkflowStore.  Note that this class should contain no tests - all the tests 
    /// are in the base class so all methods of WorkflowStore are tested consistently.
    /// </summary>
    [TestFixture]
    [Category("RequiresRavenDb")]
    public class RavenDbWorkflowStoreTest : WorkflowStoreTestBase
    {

        private EmbeddableDocumentStore _documentStore;

        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void RavenDbWorkflowStoreTest_FixtureSetUp()
        {

            // default usage: use an in-mrmory database for unit test.  Make sure you apply the ds.Convenstions.DefaultQueryingConsistency 
            // line below, otherwise you will randomly get errors when querying the store after a write
            _documentStore = new EmbeddableDocumentStore { RunInMemory = true };
            _documentStore.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            _documentStore.Initialize();

        }

        [TestFixtureTearDown]
        public void RavenDbWorkflowStoreTest_FixtureTearDown()
        {
            _documentStore.Dispose();
        }

        [SetUp]
        public void RavenDbWorkflowStoreTest_SetUp()
        {
            // make sure there is no data in the database for the next test
            ClearTestData();
        }

        [TearDown]
        public void RavenDbWorkflowStoreTest_TearDown()
        {
            // make sure there is no data in the database for the next test
            ClearTestData();
        }

        #endregion

        #region Private Methods

        private void ClearTestData()
        {
            using (IDocumentSession session = _documentStore.OpenSession())
            {
                // drop all workflows
                IEnumerable<WorkflowContainer> workflows = session.Query<WorkflowContainer>().Take(1000);
                foreach (WorkflowContainer wi in workflows)
                {
                    session.Delete(wi);
                }

                // drop all complete workflows
                IEnumerable<CompletedWorkflow> completedWorkflows = session.Query<CompletedWorkflow>().Take(1000);
                foreach (CompletedWorkflow cw in completedWorkflows)
                {
                    session.Delete(cw);
                }
                session.SaveChanges();
            }
        }

        #endregion


        #region Protected Methods

        /// <summary>
        /// Gets the store relevant to the test.
        /// </summary>
        /// <returns></returns>
        protected override IWorkflowStore GetStore()
        {
            return new RavenDbWorkflowStore(_documentStore, null);
        }

        #endregion

    }
}
