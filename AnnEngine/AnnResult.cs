namespace AnnEngine {
    public class AnnResult {
        public readonly float[] Result;
        public readonly float Error;
        
        public AnnResult(float[ ] result, float error) {
            Result = result;
            Error = error;
        }
        
    }
}