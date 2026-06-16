using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;

public class Comunicacion : MonoBehaviour
{

    TcpClient cliente = new TcpClient();
    System.Diagnostics.Stopwatch MovementPeriod = new System.Diagnostics.Stopwatch();
    string answer;

 
    bool aux = true;
    int aux2 = 200;
    // Start is called before the first frame update
    void Start()
    {
        MovementPeriod.Start();
        CREATE_CONECTION();
        

        
        //SEND_RS("Dibujar/0,101,0/0,50,0");

        //SEND_RS("Dibujar/0,102,0/0,50,0");
    }
    private async void Update()
    {
       /* if (aux)
        {
            aux= false;
            SEND_RS("Dibujar/"+aux2+" 201.5 102.4/100.2 10.5 20.3");
            aux2 += 199;
            answer = await RECIEVE_MESSAGE_RS(cliente.GetStream());
            Debug.Log(answer);
            aux = true;
        }*/
        

    }

    private void OnDestroy()
    {
        SEND_RS("STOP");
    }

    public void CREATE_CONECTION()
    {
        try
        {
            cliente.Connect("10.42.0.1", 1100);
        }
        catch (ObjectDisposedException e)
        {
            Debug.LogError("ERROR:" + e.Message);
            //CREATE_CONECTION();
            return;
        }
        catch (SocketException e)
        {
            Debug.LogError("ERROR:" + e.Message);
            //CREATE_CONECTION();
            return;
        }
        /*if (ConnectionButton.activeSelf)
        {
            ConnectionButton.SetActive(false);
        }*/

        Debug.Log("Conection established");

    }
    public void SEND_RS(string message)
    {
        try
        {
            cliente.GetStream();
        }
        catch (InvalidOperationException e)
        {
            Debug.Log("Not connected to the server. Please, connect to it");
            //ConnectionButton.SetActive(true);
        }
        SEND_MESSAGE_RS(message, cliente.GetStream());
    }
    private async void SEND_MESSAGE_RS(string message, NetworkStream stream)
    {
        try
        {
            StreamWriter writer = new StreamWriter(stream);
            await writer.WriteLineAsync(message);
            writer.Flush();
            Debug.Log("Message sent");

        }
        catch (ObjectDisposedException e)
        {
            Debug.LogError("ERROR:" + e.Message);

        }
        catch (SocketException e)
        {
            Debug.LogError("ERROR:" + e.Message);
        }

    }

    public async Task<string> RECIEVE_MESSAGE_RS()
    {
        return await RECIEVE_MESSAGE_RS(cliente.GetStream());
    }
    private async Task<string> RECIEVE_MESSAGE_RS(NetworkStream stream)
    {
        StreamReader sr = new StreamReader(stream);
        string message = null;
        message = await sr.ReadLineAsync();
        return message;
    }
    
}
