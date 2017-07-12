using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AnnEngine {
    public class Ann {
        private float[ ][ ][ ] _weights;
        private float[ ] _biasWeights;
        private float[ ][ ] _values;

        private const float MIN_START_WEIGHT = -0.5f;
        private const float MAX_START_WEIGHT = 0.5f;

        public float LearningSpeed;
        public float Moment;
        public Func<float, float> ActivationFunction;
        public Func<float, float> LearningFunction;
        public readonly bool HasBiasNeurons;

        public Ann(uint inputNeurons, uint[ ] hiddenNeurons, uint outputNeurons, float learningSpeed,
            float moment, Func<float, float> activationFunction = null, Func<float, float> learningFunction = null, bool
                useBiasNeurons = true) {
            LearningSpeed = learningSpeed;
            Moment = moment;
            ActivationFunction = activationFunction ?? Sigmoid;
            LearningFunction = learningFunction ?? SigmoidПроизводнаяУпрощённая;
            HasBiasNeurons = useBiasNeurons;
            _weights = new float[hiddenNeurons.Length + 1][ ][ ];
            _values = new float[hiddenNeurons.Length + 2][ ];
            _weights[0] = new float[inputNeurons][ ];
            _values[0] = new float[inputNeurons];
            for (uint i = 0; i < _weights[0].Length; i++) {
                _weights[0][i] = new float[hiddenNeurons[0]];
            }
            _values[_values.Length - 1] = new float[outputNeurons];
            for (uint i = 0, iNext = 1; i < hiddenNeurons.Length; i++, iNext++) {
                _values[iNext] = new float[hiddenNeurons[i]];
                _weights[iNext] = new float[hiddenNeurons[i]][ ];
                for (uint j = 0; j < hiddenNeurons[i]; j++) {
                    _weights[iNext][j] =
                        new float[(iNext == hiddenNeurons.Length) ? outputNeurons : hiddenNeurons[iNext]];
                }
            }
            for (uint i = 0; i < _weights.Length; i++) {
                for (uint j = 0; j < _weights[i].Length; j++) {
                    for (uint k = 0; k < _weights[i][j].Length; k++) {
                        _weights[i][j][k] = Utils.Random(MIN_START_WEIGHT, MAX_START_WEIGHT);
                    }
                }
            }
            if (useBiasNeurons) {
                _biasWeights = new float[hiddenNeurons.Length + 1];
                for (uint i = 0; i < _biasWeights.Length; i++) {
                    _biasWeights[i] = Utils.Random(MIN_START_WEIGHT, MAX_START_WEIGHT);
                }
            }
        }

        public float[ ] Run(float[ ] input) {
            // TODO: throw input.Length != input neurons count
            for (uint i = 0; i < input.Length; i++) {
                _values[0][i] = input[i];
            }
            for (uint currentLevel = 1, prevLevel = 0;
                currentLevel < _values.Length;
                currentLevel++, prevLevel++) {
                for (uint currentNeuron = 0; currentNeuron < _values[currentLevel].Length; currentNeuron++) {
                    _values[currentLevel][currentNeuron] = 0f;
                    for (uint commingNeuron = 0; commingNeuron < _values[prevLevel].Length; commingNeuron++) {
                        _values[currentLevel][currentNeuron] +=
                            _values[prevLevel][commingNeuron] * _weights[prevLevel][commingNeuron][currentNeuron];
                    }
                    if (HasBiasNeurons) {
                        _values[currentLevel][currentNeuron] -= _biasWeights[prevLevel];
                    }
                    _values[currentLevel][currentNeuron] = ActivationFunction(_values[currentLevel][currentNeuron]);
                }
            }
            return _values[_values.Length - 1];
        }

        public AnnResult Learn(float[ ] input, float[ ]idealResult) {
            // TODO: throw input.Length != input neurons count || idealResult.Length != output neurons count
            float[ ] result = Run(input);
            // Calculation the error
            float error = 0;
            for (uint i = 0; i < result.Length; i++) {
                error += (float) Math.Pow(idealResult[i] - result[i], 2);
            }
            error /= result.Length;
            float[ ] delta = new float[result.Length];
            for (uint i = 0; i < delta.Length; i++) {
                delta[i] = (idealResult[i] - result[i]) * LearningFunction(result[i]);
            }
//            float[ ] newDelta;
//            for (uint i = (uint) _weights.Length - 1; i > 0; i--) {
//                newDelta = new float[_weights[i].Length];
//                for (uint j = 0; j < newDelta.Length; j++) {
//                    newDelta[j] = LearningFunction( )
//                }
//            }
            return new AnnResult(result, error);
        }

        public static float Sigmoid(float x) => (float) (1f / (1f + Math.Exp(-x)));

        // TODO: rename
        public static float SigmoidПроизводнаяУпрощённая(float x) => (1 - x) * x;
    }
}