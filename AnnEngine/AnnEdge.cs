namespace AnnEngine {
    internal class AnnEdge {

        public float Weight;
        public float Gradient = 0;
        public float Delta = 0;

        public AnnEdge(float weight) {
            Weight = weight;
        }

    }
}