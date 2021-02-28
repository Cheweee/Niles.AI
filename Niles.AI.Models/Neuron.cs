using System;
using System.Collections.Generic;
///<summary> Модель нейрона </summary>
public class Neuron
{
    public int Id { get; set; }
    ///<summary> Дендриты нейрона </summary>
    public List<Synapse> Dendrites { get; set; } = new List<Synapse>();

    ///<summary> Аксон нейрона </summary>
    public double Axon { get; set; }

    public double Delta { get; set; }

    public bool IsHidden { get; set; }
}