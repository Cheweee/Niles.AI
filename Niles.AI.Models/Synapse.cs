///<summary> Модель синапса для организации связи нейронов в сеть </summary>
public class Synapse
{
    ///<summary> Вес синапса </summary>
    public double Weight { get; set; }

    ///<summary> Принимаемый импульс </summary>
    public Neuron Input { get; set; }

    ///<summary> Градиент синапса </summary>
    public double Gradient { get; set; }

    ///<summary> Дельта синапса </summary>
    public double Delta { get; set; }
    public double PreviousDelta = 0;
}

///<summary> Передаваемый синапсом пульс </summary>
public class Pulse
{
    ///<summary> Передаваемое пульсом значение </summary>
    public double Value { get; set; }
}