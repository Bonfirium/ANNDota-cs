using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AnnEngine {
    public class Ann {
        private readonly AnnEdge[ ][ ][ ] _edges;
        private readonly AnnEdge[ ][ ] _biasEdges;
        // TODO: make diff types of neurons
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
            _edges = new AnnEdge[hiddenNeurons.Length + 1][ ][ ];
            _neurons = new Neuron[hiddenNeurons.Length + 2][ ];
            _edges[0] = new AnnEdge[inputNeurons][ ];
            _neurons[0] = new Neuron[inputNeurons];
            for (uint i = 0; i < _edges[0].Length; i++) {
                _edges[0][i] = new AnnEdge[hiddenNeurons[0]];
            }
            _neurons[_neurons.Length - 1] = new Neuron[outputNeurons];
            for (uint i = 0, iNext = 1; i < hiddenNeurons.Length; i++, iNext++) {
                _neurons[iNext] = new Neuron[hiddenNeurons[i]];
                _edges[iNext] = new AnnEdge[hiddenNeurons[i]][ ];
                for (uint j = 0; j < hiddenNeurons[i]; j++) {
                    _edges[iNext][j] =
                        new AnnEdge[(iNext == hiddenNeurons.Length) ? outputNeurons : hiddenNeurons[iNext]];
                }
            }
            for (uint i = 0; i < _edges.Length; i++) {
                for (uint j = 0; j < _edges[i].Length; j++) {
                    for (uint k = 0; k < _edges[i][j].Length; k++) {
                        _edges[i][j][k] = new AnnEdge(Utils.Random(MIN_START_WEIGHT, MAX_START_WEIGHT));
                    }
                }
            }
            if (useBiasNeurons) {
                _biasEdges = new AnnEdge[hiddenNeurons.Length + 1][ ];
                for (uint i = 0; i < _biasEdges.Length; i++) {
                    _biasEdges[i] = new AnnEdge[_neurons[i + 1].Length];
                    for (uint j = 0; j < _biasEdges[i].Length; j++) {
                        _biasEdges[i][j] = new AnnEdge(Utils.Random(MIN_START_WEIGHT, MAX_START_WEIGHT));
                    }
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
                            _edges[prevLevel][commingNeuron][currentNeuron].Weight;
                    }
                    if (HasBiasNeurons) {
                        _neurons[currentLevel][currentNeuron].Value -= _biasEdges[prevLevel][currentNeuron].Weight;
                    }
                    _neurons[currentLevel][currentNeuron].Value =
                        ActivationFunction(_neurons[currentLevel][currentNeuron].Value);
                }
            }
            uint outputLevelIndex = (uint) _neurons.Length - 1;
            float[ ] result = new float[_neurons[outputLevelIndex].Length];
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
            for (uint i = 0, outputLevelIndex = (uint) _neurons.Length - 1;
                i < _neurons[outputLevelIndex].Length;
                i++) {
                _neurons[outputLevelIndex][i].Delta = (idealResult[i] - result[i]) * LearningFunction(result[i]);
            }
            for (uint i = (uint) _neurons.Length - 2, iInc = i + 1; i > 0; i--, iInc--) {
                for (uint j = 0; j < _neurons[i].Length; j++) {
                    _neurons[i][j].Delta = 0;
                    for (uint k = 0; k < _neurons[iInc].Length; k++) {
                        _neurons[i][j].Delta += _neurons[iInc][k].Delta * _edges[i][j][k].Weight;
                    }
                    _neurons[i][j].Delta *= LearningFunction(_neurons[i][j].Value);
                }
            }
            for (uint i = 0; i < _edges.Length; i++) {
                for (uint j = 0; j < _edges[i].Length; j++) {
                    for (uint k = 0; k < _edges[i][j].Length; k++) {
                        AnnEdge edge = _edges[i][j][k];
                        edge.Gradient = _neurons[i + 1][k].Delta * _neurons[i][j].Value;
                        edge.Delta = LearningSpeed * edge.Gradient + Moment * edge.Delta;
                        edge.Weight += edge.Delta;
                    }
                }
            }
            for (uint i = 0; i < _biasEdges.Length; i++) {
                for (uint j = 0; j < _biasEdges[i].Length; j++) {
                    AnnEdge biasEdge = _biasEdges[i][j];
                    biasEdge.Gradient = -_neurons[i + 1][j].Delta;
                    biasEdge.Delta = LearningSpeed * biasEdge.Gradient + Moment * biasEdge.Delta;
                    biasEdge.Weight += biasEdge.Delta;
                }
            }
            return new AnnResult(result, error);
        }

        public static float Sigmoid(float x) => (float) (1f / (1f + Math.Exp(-x)));

        // TODO: rename
        public static float SigmoidПроизводнаяУпрощённая(float x) => (1 - x) * x;
    }
}