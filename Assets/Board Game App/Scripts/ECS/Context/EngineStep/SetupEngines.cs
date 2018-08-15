using ECS.Context.EngineStep.Create;
using Svelto.ECS;
using System.Collections.Generic;

namespace ECS.Context.EngineStep
{
    public class SetupEngines
    {
        private EnginesRoot enginesRoot;
#pragma warning disable IDE0044 // Add readonly modifier
        private IEntityFactory entityFactory;

        private Dictionary<string, Sequencer> sequences = new Dictionary<string, Sequencer>();
        private Dictionary<string, IStep[]> steps = new Dictionary<string, IStep[]>();
        private Dictionary<string, IEngine> engines = new Dictionary<string, IEngine>();
#pragma warning restore IDE0044 // Add readonly modifier

        private SetupSequence setupSequence;
        private CreateAddEngine createAddEngine;
        private SetupStep setupStep;

        public SetupEngines(EnginesRoot enginesRoot, IEntityFactory entityFactory)
        {
            this.enginesRoot = enginesRoot;
            this.entityFactory = entityFactory;
            setupStep = new SetupStep(steps, engines);
            setupSequence = new SetupSequence(sequences, steps, engines);
            createAddEngine = new CreateAddEngine(enginesRoot, engines, sequences);
        }

        public void Setup()
        {
            //the ISequencer is one of the 2 official ways available in Svelto.ECS 
            //to communicate. They are mainly used for two specific cases:
            //1) specify a strict execution order between engines (engine logic
            //is executed horizontally instead than vertically, I will talk about this
            //in my articles). 2) filter a data token passed as parameter through
            //engines. The ISequencer is also not the common way to communicate
            //between engines
            //...Then what is the common way to communicate between engines?  Querying entities?
            setupSequence.CreateSequences();
            createAddEngine.CreateEngines();
            setupStep.Create();
            setupSequence.SetSequences();
            createAddEngine.AddEngines();
        }
    }
}
