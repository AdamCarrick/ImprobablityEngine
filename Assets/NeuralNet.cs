using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NN
{
    public class NeuralNet
    {

        public const double ERROR_THRESHOLD = 0.003;

        public class CNeuron
        {
            public int m_iNumInputs;
            public List<double> m_listWeight;
            public double m_dActivation;
            public double m_dError;

            public CNeuron(int NumInputs)
            {
                m_listWeight= new List<double>();
                m_iNumInputs = NumInputs + 1;
                m_dActivation = 0;
                m_dError = 0;

                for (int i = 0; i < NumInputs+1; i++)
                {
                    
                    m_listWeight.Add(Random.value);
                    //m_listWeight.Add(0.5f);
                }
            }

        };

        public class CNeuronLayer
        {
            public int m_iNumNeurons;
            public int m_iNumInputs;
            public List<CNeuron> m_listNeurons;

            public CNeuronLayer(int NumNeurons, int NumInputsPerNeuron)
            {
                m_listNeurons = new List<CNeuron>();
                m_iNumNeurons = NumNeurons;
                m_iNumInputs = NumInputsPerNeuron;
                for (int i = 0; i < m_iNumNeurons; ++i)
                {
                    m_listNeurons.Add(new CNeuron(NumInputsPerNeuron));
                }

            }
        };

        public int m_iNumInputs;
        public int m_iNumOutputs;
        public int m_iNumHiddenLayers;
        public int m_iNeuronsPerHiddenLyr;
        public double m_dLearningRate;
        public double m_dErrorSum;
        public bool m_bTrained;
        public int m_iNumEpochs;

        //storage for each layer of neurons including the output layer
        public List<CNeuronLayer> m_listLayers;

        public NeuralNet(int NumInputs, int NumOutputs, int HiddenNeurons, int LearningRate)
        {
            m_iNumInputs = NumInputs;
            m_iNumOutputs = NumOutputs;
            m_iNumHiddenLayers = 1; 
            m_iNeuronsPerHiddenLyr = HiddenNeurons;
            m_dLearningRate = LearningRate;
            m_dErrorSum = 99999;
            m_bTrained = false;
            m_iNumEpochs = 0;
            m_listLayers = new List<CNeuronLayer>();
            CreateNet();
            InitializeNetwork();
        }

        public void CreateNet()
        {
            //create the layers of the network
            if (m_iNumHiddenLayers > 0)
            {
                //create first hidden layer
                m_listLayers.Add(new CNeuronLayer(m_iNeuronsPerHiddenLyr, m_iNumInputs));

                for (int i = 0; i < m_iNumHiddenLayers - 1; ++i)
                {
                    m_listLayers.Add(new CNeuronLayer(m_iNeuronsPerHiddenLyr, m_iNeuronsPerHiddenLyr));
                }

                //create output layer
                m_listLayers.Add(new CNeuronLayer(m_iNumOutputs, m_iNeuronsPerHiddenLyr));
            }

            else
            {
                //create output layer
                m_listLayers.Add(new CNeuronLayer(m_iNumOutputs, m_iNumInputs));
            }
        }

       public void InitializeNetwork()
        {
            //for each layer
            for (int i = 0; i < m_iNumHiddenLayers; ++i)
            {
                //for each neuron
               // CNeuronLayer test = m_listLayers[1];
                //CNeuronLayer sample = new CNeuronLayer(12, 12);

                //Debug.Log("Inside i For i = " + i);
                //Debug.Log("Target number " + m_listLayers[1].m_iNumNeurons + " Loops");
                for (int n = 0; n < m_listLayers[i].m_iNumNeurons; ++n)
                {
                    //for each weight
                    for (int k = 1; k < m_listLayers[i].m_listNeurons[n].m_iNumInputs; ++k)
                    {
                        
                        m_listLayers[i].m_listNeurons[n].m_listWeight[k] = Random.value;
                    }
                }
            }

            m_dErrorSum = 9999;
            m_iNumEpochs = 0;
            //Debug.Log("Exited The Init Function");
            return;
        }

        public List<double> UpdateNN(List<double> inputs)
        {
            //stores the resultant outputs from each layer
            List<double> outputs = new List<double>();

            int cWeight = 0;

            //first check that we have the correct amount of inputs
            if (inputs.Count != m_iNumInputs)
            {
                //just return an empty vector if incorrect.
                return (outputs);
            }

            //For each layer...
            for (int i = 0; i < m_iNumHiddenLayers + 1; ++i)
            {

                if (i > 0)
                {
                    inputs = new List<double>(outputs);
                    //outputs.CopyTo(inputs);
                    //inputs = outputs
                }

                outputs.Clear();

                cWeight = 0;

                //for each neuron sum the (inputs * corresponding weights).Throw 
                //the total at our sigmoid function to get the output.
                for (int n = 0; n < m_listLayers[i].m_iNumNeurons; ++n)
                {
                    double netinput = 0;

                    int NumInputs = m_listLayers[i].m_listNeurons[n].m_iNumInputs;

                    //for each weight
                    for (int k = 0; k < NumInputs - 1; ++k)
                    {
                        //sum the weights x inputs
                        netinput += m_listLayers[i].m_listNeurons[n].m_listWeight[k] *
                            inputs[cWeight++];
                      //  Debug.Log(i);
                      //  Debug.Log(n);
                      //  Debug.Log(k);
                      //  Debug.Log(cWeight);
                    }

                    //add in the bias
                    netinput += m_listLayers[i].m_listNeurons[n].m_listWeight[NumInputs - 1] * 1;//BIAS;

                    //The combined activation is first filtered through the sigmoid 
                    //function and a record is kept for each neuron 
                    m_listLayers[i].m_listNeurons[n].m_dActivation = Sigmoid(netinput, 1);

                    //store the outputs from each layer as we generate them.
                    outputs.Add(m_listLayers[i].m_listNeurons[n].m_dActivation);

                    cWeight = 0;
                }
            }

            return outputs;
        }


       public bool NetworkTrainingEpoch(List<List<double>> SetIn, List<List<double>> SetOut)
        {
            //create some iterators
            List<double>.Enumerator curWeight;
            List<CNeuron>.Enumerator curNrnOut, curNrnHid;
            // double curWeight;
            // int curNrnOut,curNrnHid;

            //this will hold the cumulative error value for the training set
            m_dErrorSum = 0;

            //run each input pattern through the network, calculate the errors and update
            //the weights accordingly
            for (int vec = 0; vec < SetIn.Count; ++vec)
            {
                //first run this input vector through the network and retrieve the outputs
                List<double> outputs = new List<double>(SetIn[vec]);
                //List<double> outputs = UpdateNN(SetIn[vec]);

                //return if error has occurred
                if (outputs.Count == 0)
                {
                    return false;
                }

                //for each output neuron calculate the error and adjust weights
                //accordingly
                for (int op = 0; op < m_iNumOutputs; ++op)
                {
                    //first calculate the error value
                    double err = (SetOut[vec][op] - outputs[op]) * outputs[op]
                                 * (1 - outputs[op]);

                    //keep a record of the error value
                    m_listLayers[1].m_listNeurons[op].m_dError = err;

                    //update the SSE. (when this value becomes lower than a
                    //preset threshold we know the training is successful)
                    m_dErrorSum += (SetOut[vec][op] - outputs[op]) *
                                   (SetOut[vec][op] - outputs[op]);

                    curWeight = m_listLayers[1].m_listNeurons[op].m_listWeight.GetEnumerator();

                    curNrnHid = m_listLayers[0].m_listNeurons.GetEnumerator();
                    //In C# we need to Move to the first element as the enumerator starts at -1
                    curNrnHid.MoveNext();
                    curWeight.MoveNext();

                    //for each weight up to but not including the bias
                    while (curWeight.Current != m_listLayers[1].m_listNeurons[op].m_listWeight[m_listLayers[1].m_listNeurons[op].m_listWeight.Count - 1])
                    {
                        //This is some of the most hideous code i have ever mashed together, I wouldn't even try to make sense of it if i didn't have to
                        //

                        //calculate the new weight based on the back prop rules
                        //curWeight.Current += err * m_dLearningRate * curNrnHid.Current.m_dActivation;
                        //Debug.Log("Index of  = " + m_listLayers[1].m_listNeurons[op].m_listWeight.IndexOf(curWeight.Current));
                        m_listLayers[1].m_listNeurons[op].m_listWeight[m_listLayers[1].m_listNeurons[op].m_listWeight.IndexOf(curWeight.Current)] += err * m_dLearningRate * curNrnHid.Current.m_dActivation;
                        // m_listLayers[1].m_listNeurons[op].m_listWeight.Equals(curWeight.Current);
                        //+= err * m_dLearningRate * curNrnHid.Current.m_dActivation;
                        curWeight.MoveNext();
                        curNrnHid.MoveNext();

                    }

                    //and the bias for this neuron
                    m_listLayers[1].m_listNeurons[op].m_listWeight[m_listLayers[1].m_listNeurons[op].m_listWeight.IndexOf(curWeight.Current)] += err * m_dLearningRate * 1;//BIAS;

                }

                //**moving backwards to the hidden layer**
                curNrnHid = m_listLayers[0].m_listNeurons.GetEnumerator();

                int n = 0;
                curNrnHid.MoveNext();
                //for each neuron in the hidden layer calculate the error signal
                //and then adjust the weights accordingly
                while (curNrnHid.Current != m_listLayers[0].m_listNeurons[m_listLayers[0].m_listNeurons.Count-1])
                {
                    double err = 0;

                    curNrnOut = m_listLayers[1].m_listNeurons.GetEnumerator();
                    curNrnOut.MoveNext();
                    //to calculate the error for this neuron we need to iterate through
                    //all the neurons in the output layer it is connected to and sum
                    //the error * weights
                    while (curNrnOut.Current != m_listLayers[1].m_listNeurons[m_listLayers[1].m_listNeurons.Count-1])
                    {
                        err += curNrnOut.Current.m_dError * curNrnOut.Current.m_listWeight[n];

                        curNrnOut.MoveNext();
                    }

                    //now we can calculate the error
                    err *= curNrnHid.Current.m_dActivation * (1 - curNrnHid.Current.m_dActivation);
                    
                    Debug.Log(err);
                    
                    //for each weight in this neuron calculate the new weight based
                    //on the error signal and the learning rate
                    for (int w = 0; w < m_iNumInputs; ++w)
                    {
                        //calculate the new weight based on the back prop rules
                        curNrnHid.Current.m_listWeight[w] += err * m_dLearningRate * SetIn[vec][w];
                    }

                    //and the bias
                    curNrnHid.Current.m_listWeight[m_iNumInputs] += err * m_dLearningRate * 1;//BIAS;

                    curNrnHid.MoveNext();
                    ++n;
                }

            }//next input vector
            return true;
        }
        /**
            bool Train()
            {
            List<List<double> > SetIn  = data->GetInputSet();
            List<List<double> > SetOut = data->GetOutputSet();

           //first make sure the training set is valid
           if ((SetIn.Count     != SetOut.Count)  || 
               (SetIn[0].Count  != m_iNumInputs)   ||
               (SetOut[0].Count != m_iNumOutputs))
           {
             Debug.Log( "Inputs != Outputs Error");
	
             return false;
           }
  
           //initialize all the weights to small random values
           InitializeNetwork();

           //train using back prop until the SSE is below the user defined
           //threshold
           while( m_dErrorSum > ERROR_THRESHOLD )
           {
             //return false if there are any problems
             if (!NetworkTrainingEpoch(SetIn, SetOut))
             {
               return false;
             }

             ++m_iNumEpochs;

           }

           m_bTrained = true;
   
           return true;
        }*/

        //gets the weights for the NN
        //	List<double> GetWeights(){}

        //returns total number of weights in net
        //int GetNumberOfWeights(){}

        //replaces the weights with new ones
        //void PutWeights(vector<double> &weights){}

        //calculates the outputs from a set of inputs
        //List<double>	Update(List<double> &inputs){}

        //sigmoid response curve
        public double Sigmoid(double activation, double response)
        {
            return (1 / (1 + Mathf.Exp((float)(activation / response))));
        }



    }
}