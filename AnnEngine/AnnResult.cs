namespace AnnEngine {
    public class AnnResult {
        public readonly float[] Result;
        public readonly float Error;
        
        internal AnnResult(float[ ] result, float error) {
            Result = result;
            Error = error;
        }
        
    }
}