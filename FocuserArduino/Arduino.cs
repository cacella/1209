using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandMessenger;
using CommandMessenger.TransportLayer;

namespace ASCOM.DogsHeaven
{
    enum Command
    {
       Move,
       Halt,
       Acknowledge
    };

    public class Arduino
    {

        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        public string status;
        ReceivedCommand rcmd = new ReceivedCommand();
        private ASCOM.Utilities.TraceLogger tl;


        // Setup function
        public void Setup(string comm, ASCOM.Utilities.TraceLogger tr)
        {
            tl = tr;
            // Create Serial Port object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = comm, BaudRate = 115200, DtrEnable = false } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport)
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            };

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Attach to NewLinesReceived for logging purposes
         //   _cmdMessenger.NewLineReceived += NewLineReceived;

            // Attach to NewLineSent for logging purposes
       //     _cmdMessenger.NewLineSent += NewLineSent;

            // Start listening
            _cmdMessenger.StartListening();
        }

        // Exit function
        public void Exit()
        {
            // Stop listening
            _cmdMessenger.StopListening();

            // Dispose Command Messenger
            _cmdMessenger.Dispose();

            // Dispose Serial Port object
            _serialTransport.Dispose();

            // Pause before stop
            //      Console.WriteLine("Press any key to stop...");
            //    Console.ReadKey();
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Move, OnMove);
            _cmdMessenger.Attach((int)Command.Acknowledge, OnAcknowledge);
        }

        public string Move(int mov)
        {
            tl.LogMessage("Transmitting", "");
            SendCommand command = new SendCommand((int)Command.Move, (int)Command.Acknowledge, 2000);
            command.AddArgument(mov);
            rcmd = _cmdMessenger.SendCommand(command);
            if (rcmd.Ok)
            {
                tl.LogMessage("Transmit", "true");
                System.Threading.Thread.Sleep(mov);
            }
            else
            {
                tl.LogMessage("Transmit", "false");
            }
            
            return status;
        }

        public string Halt()
        {
            tl.LogMessage("Transmitting", "");
            SendCommand command = new SendCommand((int)Command.Halt, (int)Command.Acknowledge, 2000);
            command.AddArgument(0);
            rcmd = _cmdMessenger.SendCommand(command);
            if (rcmd.Ok)
            {
                tl.LogMessage("Transmit", "true");
   //             System.Threading.Thread.Sleep(mov);
            }
            else
            {
                tl.LogMessage("Transmit", "false");
            }

            return status;
        }

        // Callback function that prints that the Arduino has acknowledged
        void OnAcknowledge(ReceivedCommand arguments)
        {
            Console.WriteLine("Ack");
            status = rcmd.RawString;
        }


        void OnMove(ReceivedCommand arguments)
        {
            Console.WriteLine("Ack");
            status = rcmd.RawString;
        }


        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received");
        }

        // Callback function that prints that the Arduino has experienced an error
        public string Ack()
        {
            SendCommand command = new SendCommand((int)Command.Acknowledge, (int)Command.Acknowledge, 2000);
            rcmd = _cmdMessenger.SendCommand(command);
            if (rcmd.Ok)
            {
            }
            return status;
            //{
        }

  
    }

}
