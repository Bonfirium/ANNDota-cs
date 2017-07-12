using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AnnEngine {
    public class Ann {
        private readonly float[ ][ ][ ] _weights;
        private readonly float[ ] _biasWeights;
        private readonly Neuron[ ][ ] _neurons;

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
            _neurons = new Neuron[hiddenNeurons.Length + 2][ ];
            _weights[0] = new float[inputNeurons][ ];
            _neurons[0] = new Neuron[inputNeurons];
            for (uint i = 0; i < _weights[0].Length; i++) {
                _weights[0][i] = new float[hiddenNeurons[0]];
            }
            _neurons[_neurons.Length - 1] = new Neuron[outputNeurons];
            for (uint i = 0, iNext = 1; i < hiddenNeurons.Length; i++, iNext++) {
                _neurons[iNext] = new Neuron[hiddenNeurons[i]];
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
            for (uint i = 0; i < _neurons.Length; i++) {
                for (uint j = 0; j < _neurons[i].Length; j++) {
                    _neurons[i][j] = new Neuron( );
                }
            }
        }

        public float[ ] Run(float[ ] input) {
            // TODO: throw input.Length != input neurons count
            for (uint i = 0; i < input.Length; i++) {
                _neurons[0][i].Value = input[i];
            }
            for (uint currentLevel = 1, prevLevel = 0;
                currentLevel < _neurons.Length;
                currentLevel++, prevLevel++) {
                for (uint currentNeuron = 0; currentNeuron < _neurons[currentLevel].Length; currentNeuron++) {
                    _neurons[currentLevel][currentNeuron].Value = 0f;
                    for (uint commingNeuron = 0; commingNeuron < _neurons[prevLevel].Length; commingNeuron++) {
                        _neurons[currentLevel][currentNeuron].Value +=
                            _neurons[prevLevel][commingNeuron].Value *
                            _weights[prevLevel][commingNeuron][currentNeuron];
                    }
                    if (HasBiasNeurons) {
                        _neurons[currentLevel][currentNeuron].Value -= _biasWeights[prevLevel];
                    }
                    _neurons[currentLevel][currentNeuron].Value =
                        ActivationFunction(_neurons[currentLevel][currentNeuron].Value);
                }
            }
            uint outputLevelIndex = (uint)_neurons.Length - 1;
            float[] result = new float[_neurons[outputLevelIndex].Length];
            for (uint i = 0; i < result.Length; i++) {
                result[i] = _neurons[outputLevelIndex][i].Value;
            }
            return result;
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
            return new AnnResult(result, error);
        }

        public static float Sigmoid(float x) => (float) (1f / (1f + Math.Exp(-x)));

        // TODO: rename
        public static float SigmoidПроизводнаяУпрощённая(float x) => (1 - x) * x;
    }
}