namespace FogSystems
{
    /// <summary>
    /// Use this base abstract class to create before or after effect expansions to the Fog System. 
    /// </summary>
    internal abstract class FogGenerator
    {
        public virtual void GenerateFog()
        {
            // Called by each fog generator before the generation begins.
        }

        public virtual void FinaliseFogGeneration()
        {
            // Called at the end of every fog generation cycle.
        }

        public virtual void RemoveFog()
        {
            // Called by each fog generator before the fog removal begins.
        }

        // This method is called after fog has been removed. 
        public abstract void FinaliseFogRemoval();

        // Some generators leave artifacts behind. This function is called by fog generators that require cleanup after removal. 
        public abstract void CleanupArtifacts();
    }
}