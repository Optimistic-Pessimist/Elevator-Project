using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;


namespace PROJEKT_FINAL
{
    public partial class Form1 : Form
    {
        /* Creating needed variables for movement (Max and min levels of reach that can be changed easily depending on number
         * of floors and calling context into the form which will allow me to connect with it*/
         Context context;
         int DoorMaxLeft = 300;
         int DoorMaxRight = 446;
         int FirstFloorLevel = 257;
         int SecondFloorLevel = 55;
         int DoorsLeftMin = 348;
         int DoorsRightMin = 397;
         private string dbconn = "Provider=Microsoft.ACE.OLEDB.12.0;" + @"data source = ../../Db/CSharpDB.accdb"; // Setting up a relative path for a DataBase
         private string dbcommand = "Select * from Elevator";
         private OleDbDataAdapter adapter;
         private OleDbCommandBuilder builder;
         DataSet ds = new DataSet();
         



        public Form1()
        {
           InitializeComponent();                          //Visual studio uses it to define everything in the form                                    
           context = new Context(new Floor_0(), this);          // Setting up default state

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Notifications();                                   // Setting notifications in availible text boxes/labels to inform customer of current elevator state
            UpdatePanel();                                            // Updating inside panel with current floor/movement
            OleDbConnection conn = new OleDbConnection(dbconn);
            OleDbCommand comm = new OleDbCommand(dbcommand, conn);
            adapter = new OleDbDataAdapter(comm);
            builder = new OleDbCommandBuilder(adapter);
            
            LoadFromDb();
            
        }
        // A simple function that allows me to put things at pause when needed. Used for keeping the sequence of opening/closing doors on pause before starting closing it
        async Task Wait()
        {
            await Task.Delay(2000);
        }

        

        /// ////////////////////////////////////////////
        /// [Event] and [Time] - rows // Elevator - name of db table // Lbox - My list
        /// 
        public void IfConnected()
        {
            try
            {


            }

            catch
            {


            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            SaveToDb();
        }

        private void ClrLog_Click(object sender, EventArgs e)
        {
            DataRow newRow = ds.Tables[0].NewRow();
            newRow["TimeDate"] = DateTime.Now;
            newRow["EventEl"] = "Manos is a cool guy";

            ds.Tables[0].Rows.Add(newRow);

            


                
        }
        private void UpLog_Click(object sender, EventArgs e)
        {
            UpdateList();
        }
        public void UpdateList()
        {
            Lbox.Items.Clear();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                    Lbox.Items.Add(row["TimeDate"] + " || The elevator " + row["EventEl"] + " from G floor " );
            }


        }
        public void SaveToDb()                                                                 // save into the database function
        {
            int updated = 0;                                                               // var for number of rows that will be updated
            if (ds.Tables[0].Rows.Count != 0)                                              // checks if the dataset is already initialized
            {

                try
                {
                    DataSet dataSetChanges = ds.GetChanges();                              //
                    updated = adapter.Update(dataSetChanges);                              // saves in the database what exists in the dataset
                    ds.Tables[0].AcceptChanges();                                          //
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            if (updated < 1)                                                                // the database is not updated as expected due to unexpected reasons
            {
                ReloadFromDb();                                               
            }
        }
        
        public void ReloadFromDb()                                                               // reload from the database to the dataset 
        {
            ds.Clear();                                                                    // clears the dataset
            LoadFromDb();                                                     
        }
        
        public void LoadFromDb()                                                                 // load from the database to the dataset 
        {
            try
            {
                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        ///////////////////////////////////////////////

        public void DbSave()
        {
            try
            {
               
            
            }
            catch
            {

            }
        }
        

        
        abstract class Elevator                                            // Stating my state
        {
            protected Context _state;                                     // Stating Context to the state and connection between
            public void SetContext(Context context)
            {
                this._state = context;
            }
            public abstract void MoveElUp(Context context);                 // Functions used in every state
            
            public abstract void MoveElDwn(Context context);
            
        }
        
        class Floor_0 : Elevator                                                        // First state of Elevator. Below different behaviours depending on current state active.
        {

            public override void MoveElUp(Context context)
            {

                
                if (context.CurrentFloor() == 1 && context.IAmMoving() == false)           // Making sure the movement can't start until Doors are closed and nothing is moving. Moving to another state
                {
                    context.MovingUp();
                    context._state = new Floor_1();
                    context.UpdatePanel();
                }
                else
                {

                    context.Error();                                                                   // Catching error if anything else is tried

                }


            }

            public override void MoveElDwn(Context context)
            {
                if (context.IAmMoving())
                {

                    context.Error();                                                                //Error in case of movement of any element
                }
                else
                {
                    context.MovingDown();
                }

            }



        }
        class Floor_1 : Elevator                                                     // Everything is similiar to the previous state but with adjusments to different floor which 
        {                                                                            // changes the behaviour (compared to first state) of objects based on new state
            public override void MoveElUp(Context context)
            {
                
                    if (context.IAmMoving())
                    {

                        context.Error();
                    }

                    else
                    {
                        
                        context.MovingUp();

                    }
                
                
            }
           
            public override void MoveElDwn(Context context)
            {
                if (context.CurrentFloor() == 2 && context.IAmMoving()==false )
                {
                    context._state = new Floor_0();
                    context.MovingDown();
                    context.UpdatePanel();

                }
                else 
                    {

                    context.Error();
                }


            }
            

        }
        class Context                                                                 // setting up Context for the state which helps connecting state in and out with the form and contains various functions
        {
            public Elevator _state;                                                 
            protected Form1 MiddleMan;                                                // Setting up the connection with the rest of the form

            public Context(Elevator state, Form1 form1)                              //Describing how to connect either the state , or the rest of the form  from the level of Context
            {
                _state = state;
                MiddleMan = form1;
            }
            public bool IAmMoving()                                                 // Using IfMoving function in the form checks for any movement of objects
            {
                if (MiddleMan.IfMoving())
                { return true; }
                else
                    return false;
            }
            public int CurrentFloor()
            {
                if (MiddleMan.ElevatorBox.Top == MiddleMan.FirstFloorLevel)
                {
                    int floor = 1;
                    return floor;
                }
                else
                {
                    int floor = 2;
                    return floor;
                }
            }
            /// <summary>
            /// Below there are a lot of references to different functions outside of Context. I will comment them at source
            /// </summary>
            public void UpdatePanel()
            { 
                MiddleMan.UpdatePanel();
            }
            public void Wait()
            {
                MiddleMan.Wait();

            }
            public void Error()
            {
                MiddleMan.Error();
            }

            public void MoveElUp()
            {
                _state.MoveElUp(this);
            }
            public void MoveElDwn()
            {
                _state.MoveElDwn(this);
            }

            public void OpenDoors()
            {
                MiddleMan.OpenDoors();
            }
            public void CloseDoors()
            {
                MiddleMan.CloseDoors();

            }
            public void MovingUp()
            {
                MiddleMan.MoveUp();

            }
            public void MovingDown()
            {
                MiddleMan.MoveDown();

            }
            public string CurrentState()                 // Reading the current State
            {
                return _state.GetType().Name;
                
            }
            public Elevator State                       // Setting up the state updates/changes
            {
                get { return _state; }
                set { _state = value; }
            }
        }
        

        public int StateCurrent()                  // Reading current State and returning the name of it 
        { 
            return context.CurrentFloor(); 
        }
        
        public void UpdatePanel()                                // Updating the panel in the elevator with current floor/Direction
        {
            if (ElevatorBox.Top == FirstFloorLevel)
            { t1.Text = "1";
                t1.Refresh();
            }
            else if (ElUpTmr.Enabled)
            {
                t1.Text = "1>";
                t1.Refresh();
            }
            else if (ElevatorBox.Top == SecondFloorLevel)
            {
                t1.Text = "2";
                t1.Refresh();
            }
            else if (ElDwnTmr.Enabled)
            {
                t1.Text = "<2";
                t1.Refresh();
            }
        }
        
        public void TxtUp(string z, string y)                               // Function to help print messages into textboxes visible from outside of elevator
        {
            Not1.Text = z;
            Not2.Text = y;
            Not1.Refresh();
            Not2.Refresh();
            
        }
        public async void Error()                                                // Setting up an error in case anyone tries to use any buttons when movement is already active. Informing why it can't be used
        {
            t2.Text = "The Elevator is moving!";                                  // Dissapearing after two seconds. 
            t3.Text = "The Elevator is moving!";
            await Wait();
            t2.Clear();
            t3.Clear();

        }
        
        public async void Notifications()                                     // Helping with informing the potential user of Elevator where the object is. Showing information outside the Elevator. Using TxtUp()
        {
            if (ElevatorBox.Top ==FirstFloorLevel)
            {
                TxtUp("Current Floor: 0", "Current Floor: 0");
                

            }
            else if (ElevatorBox.Top == SecondFloorLevel)
            {
                TxtUp("Current Floor: 1", "Current Floor: 1");
                

            }




            else if (ElevatorBox.Top<FirstFloorLevel && ElUpTmr.Enabled)
            {


                TxtUp("Going to floor 1!", "Coming from floor 0!");


            }
            else if (ElevatorBox.Top > SecondFloorLevel && ElDwnTmr.Enabled)
            {

                TxtUp("Coming from floor 1!","Going to floor 0!"  );



            }
        }
       

        private void Call0_Click(object sender, EventArgs e)
        {
           
            context.MoveElDwn();
        }

        private void Call1_Click(object sender, EventArgs e)
        {

            
            context.MoveElUp();

        }

        private void Floor1_Click(object sender, EventArgs e)
        {
            
            context.MoveElDwn();
        }

        private void Floor2_Click(object sender, EventArgs e)
        {
            
            context.MoveElUp();
        }

       // Functions above describe what happens when someone presses button (All of them named according to their purpose). All of them lead to final movement of Elevator Up and Down, depending on the current state.



        private void OpenDoorsB_Click(object sender, EventArgs e)             // Button for opening doors. It can be activited from inside of elevator only if Elevator is not moving. 
        {                                                                     // Firing Error whenever someone tries to use it during movement
            if (ElUpTmr.Enabled || ElUpTmr.Enabled)
            {
                Error();
            }
            else
            {
                DoorsClTmr.Stop();
                OpenDoors();
            }
        }

        private void CloseDoorsB_Click(object sender, EventArgs e)  // Same as the button above. This one is for closing doors. Firing Error whenever someone tries to use it during movement
        {
            if (ElUpTmr.Enabled || ElUpTmr.Enabled)
            {
                Error();
            }
            else
            {
                DoorsOpTmr.Stop();
                CloseDoors();
            }
        }
        public bool IfMoving()                                                          // Returning true value if any of a timers is on, false otherwise. Checking for movement
        {
            if (ElUpTmr.Enabled || ElDwnTmr.Enabled || DoorsOpTmr.Enabled || DoorsClTmr.Enabled)
            {
                return true;
            }
            else
            { 
                return false;
            }


        }

        private void PanelLoc()                                       // Making sure that the Panel in the Project desing follows Elevator movement
        {
            Panel.Top = ElevatorBox.Top;
        }
        private void ElUpTmr_Tick(object sender, EventArgs e)                      // Timer for moving elevator up. Changing colours depending on movement and updating all notification windows
        {
            if (ElevatorBox.Top==SecondFloorLevel)
            {
                Call0.BackColor = Color.Gainsboro;
                Call1.BackColor = Color.Gainsboro;
                ElUpTmr.Stop();
                UpdatePanel();
                OpenDoors();

            }
            else
            {
                Call0.BackColor = Color.Red;
                Call1.BackColor = Color.Red;
                ElevatorBox.Location = new Point(ElevatorBox.Location.X, ElevatorBox.Location.Y - 1);
                PanelLoc();
                Notifications();
                UpdatePanel();
            }
        }

        private void ElDwnTmr_Tick(object sender, EventArgs e)             // Same as above. Difference is this timer moves Elevator down the Y Axis.
        {
            if (ElevatorBox.Top == FirstFloorLevel )
            {
                Call0.BackColor = Color.Gainsboro;
                Call1.BackColor = Color.Gainsboro;
                ElDwnTmr.Stop();
                UpdatePanel();
                OpenDoors();

            }
            else 
            {
                Call0.BackColor = Color.Red;
                Call1.BackColor = Color.Red;
                ElevatorBox.Location = new Point(ElevatorBox.Location.X, ElevatorBox.Location.Y + 1);
                PanelLoc();
                Notifications();
                UpdatePanel();
            }
        }

        /// <summary>
        /// Below creating simple names for 4 functions directly connecting to timers
        /// </summary>
        private void OpenDoors()
        {
            DoorsOpTmr.Start();

        }
        private void CloseDoors()
        {
            DoorsClTmr.Start();

        }
        private void MoveUp()
        {
            ElUpTmr.Start();


        }
        private void MoveDown()
        {
            ElDwnTmr.Start();

        }
        private async void DoorsOpTmr_Tick(object sender, EventArgs e)                     // Timer for opening doors. Works for both floor at the same time. Leaves doors open for some time before 
        {                                                                                  // calling function to close them


            if (ElevatorBox.Top==FirstFloorLevel && D1l.Left>=DoorMaxLeft)
            {
                D1l.Left -= 1;
                D1r.Left += 1;
            }
            else if (ElevatorBox.Top == SecondFloorLevel && D2l.Left >= DoorMaxLeft)
            {
                D2l.Left -= 1;
                D2r.Left += 1;

            }
            else
                {

                   await Wait();
                   DoorsOpTmr.Stop();
                   
                   DoorsClTmr.Start();
               }
        }

        private void DoorsClTmr_Tick(object sender, EventArgs e)                 // Timer for closing the doors that works for all floors.
           {
              

              if (ElevatorBox.Top == FirstFloorLevel && D1l.Left <= DoorsLeftMin)
               {
                D1l.Left += 1;
                   D1r.Left -= 1;

               }
              else if (ElevatorBox.Top == SecondFloorLevel && D2l.Left <= DoorsLeftMin)
               {
                   D2l.Left += 1;
                D2r.Left -= 1;
                }
              else
               {
                DoorsClTmr.Stop();
               }

        }

        





        //private void ClrLog_Click(object sender, EventArgs e)
        //{

        //}


    }
}
