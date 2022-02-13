using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YagnaSharpApi.Engine;
using YagnaSharpApi.Engine.Commands;
using YagnaSharpApi.Engine.Events;
using YagnaSharpApi.Studio.Model;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Studio
{
    public partial class MainForm : Form
    {

        public Config Config { get; set; } = new Config();

        public BindingList<EventModel> EventList { get; set; } = new BindingList<EventModel>();
        public BindingList<OfferModel> OfferList { get; set; } = new BindingList<OfferModel>();
        public BindingList<AgreementModel> AgreementList { get; set; } = new BindingList<AgreementModel>();

        public MainForm()
        {
            InitializeComponent();

            this.eventModelBindingSource.DataSource = this.EventList;
            this.eventsDataGridView.DataSource = this.eventModelBindingSource;

            this.offerModelbindingSource.DataSource = this.OfferList;
            this.offersDataGridView.DataSource = this.offerModelbindingSource;

            this.offerEventsDataGridView.DataSource = this.offerEventBindingSource;

            this.agreementModelBindingSource.DataSource = this.AgreementList;
            this.agreementsDataGridView.DataSource = this.agreementModelBindingSource;

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.startButton.Enabled = false;
            try
            {
                await Task.Run(StartExecution);

            }
            catch(Exception exc)
            {
                System.Diagnostics.Debug.Write(exc);
                MessageBox.Show(exc.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.startButton.Enabled = true;
        }

        private async Task StartExecution()
        {
            List<GolemTask<int, string>> acceptedTasks = new List<GolemTask<int, string>>();

            async IAsyncEnumerable<Script> ProcessGolemTasksAsync(WorkContext ctx, IAsyncEnumerable<GolemTask<int, string>> tasks)
            {
                var scenePath = "Assets/cubes.blend";
                await foreach (var task in tasks)
                {
                    var frame = task.Data;
                    var script = ctx.NewScript();
                    script.SendFile(scenePath, "/golem/resource/scene.blend");
                    script.SendJson(
                        new
                        {
                            scene_file = "/golem/resource/scene.blend",
                            resolution = new[] { 40, 30 },
                            use_compositing = false,
                            crops = new[] { new { outfilebasename = "out", borders_x = new[] { 0.0, 1.0 }, borders_y = new[] { 0.0, 1.0 } } },
                            samples = 100,
                            frames = new[] { frame },
                            output_format = "PNG",
                            RESOURCES_DIR = "/golem/resources",
                            WORK_DIR = "/golem/work",
                            OUTPUT_DIR = "/golem/output"
                        },
                        "/golem/work/params.json");

                    script.Run("/golem/entrypoints/run-blender.sh");
                    var outputFile = $"output_{frame}.png";
                    script.DownloadFile($"/golem/output/out{frame:d4}.png", outputFile);
                    yield return script;
                    // TODO check if results are valid
                    task.AcceptTask(outputFile);
                    acceptedTasks.Add(task);
                }

            }

            // one data set
            var data = Enumerable.Range(0, 1).Select(item => new GolemTask<int, string>(item * 10)).ToList();

            await this.DoExecutorRunAsync(ProcessGolemTasksAsync, data, 
            600);

        }

        protected async Task DoExecutorRunAsync<Input, Output>(
            Func<WorkContext, IAsyncEnumerable<GolemTask<Input, Output>>, IAsyncEnumerable<Script>> workerFunc,
            IEnumerable<GolemTask<Input, Output>> data,
            int timeoutSeconds = 360)
        {
            var package = VmRequestBuilder.Repo(
                "9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae",
                0.5m,
                2.0m
                );


            using (var executor = new Engine.Executor(
                package,
                1,
                data.Count() * 5,
                timeoutSeconds,
                this.Config.SubnetTag))
            {
                executor.OnExecutorEvent += Executor_OnExecutorEvent;
                var inputTasks = data.ToList();

                await foreach (var task in executor.SubmitAsync(workerFunc, inputTasks))
                {
                    Console.WriteLine($"{TextColorConstants.TEXT_COLOR_CYAN}Task computed: {task}, result: {task.Result}{TextColorConstants.TEXT_COLOR_DEFAULT}");
                }
            }

        }

        private void Executor_OnExecutorEvent(object sender, Engine.Events.Event e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.EventList.Add(new EventModel(e));

                this.HandleEvent(e);
            });
            
        }

        private ProposalStore proposalStore = new ProposalStore();

        protected void HandleEvent(Event ev)
        {
            OfferModel HandleProposalEvent(ProposalEvent pev)
            {
                var rootProposal = this.proposalStore.GetRootProposal(pev.Proposal);

                var rootOfferModel = this.OfferList.FirstOrDefault(offerModel => offerModel.Id == rootProposal.ProposalId);

                rootOfferModel.Events.Add(pev);
                // TODO: any other actions on the root Offer Model record

                return rootOfferModel;
            }

            switch (ev)
            {
                case ProposalReceived pr:
                    this.proposalStore.AddProposal(pr.Proposal);

                    if (pr.Proposal.PrevProposalId == null) // original offer
                        this.OfferList.Add(new OfferModel(pr.Proposal));
                    else
                        HandleProposalEvent(pr);
                    break;
                case ProposalResponded presp:
                    this.proposalStore.AddProposal(presp.CounterProposal);
                    HandleProposalEvent(presp);
                    break;
                case ProposalRejected pr:
                    HandleProposalEvent(pr);
                    break;
                case ProposalFailed pr:
                    HandleProposalEvent(pr);
                    break;
                case AgreementCreated ac:
                    var rootProposal = this.proposalStore.GetRootProposal(ac.OfferProposal);
                    var rootOfferModel = this.OfferList.FirstOrDefault(offerModel => offerModel.Id == rootProposal.ProposalId);
                    rootOfferModel.Events.Add(ac);

                    var agreementModel = new AgreementModel(ac.Agreement);
                    agreementModel.Events.Add(ac);
                    this.AgreementList.Add(agreementModel);

                    break;

                case AgreementEvent ae:
                    var agrModel = this.AgreementList.FirstOrDefault(agr => agr.Agreement.AgreementId == ae.Agreement.AgreementId);
                    agrModel.Events.Add(ae);
                    break;

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void eventModelBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void eventsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.eventDetailTextBox.Text = String.Empty;
            this.eventDetailsTreeView.Nodes.Clear();

            foreach (var row in this.eventsDataGridView.SelectedRows)
            {
                var selectedItem = (row as DataGridViewRow).DataBoundItem as EventModel;

                // populate textbox
                var text = JsonConvert.SerializeObject(selectedItem.Event, Formatting.Indented);

                this.eventDetailTextBox.Text = text;

                // populate treeview

                var jsonObject = JObject.Parse(text);

                var rootNode = this.Json2Tree(jsonObject, selectedItem.Event.GetType().Name);

                this.eventDetailsTreeView.Nodes.Add(rootNode);

                break;
            }



        }

        private TreeNode Json2Tree(JObject obj, string key)
        {
            //create the parent node
            TreeNode parent = new TreeNode();
            parent.Text = key;

            //loop through the obj. all token should be pair<key, value>
            foreach (var token in obj)
            {
                //create the child node
                TreeNode child = new TreeNode();
                //check if the value is of type obj recall the method
                if (token.Value.Type.ToString() == "Object")
                {
                    // child.Text = token.Key.ToString();
                    //create a new JObject using the the Token.value
                    JObject o = (JObject)token.Value;
                    //recall the method
                    child = Json2Tree(o, token.Key);
                    //add the child to the parentNode
                    parent.Nodes.Add(child);
                }
                //if type is of array
                else if (token.Value.Type.ToString() == "Array")
                {
                    int ix = -1;
                    child.Text = $"{token.Key}";
                    //loop though the array
                    foreach (var itm in token.Value)
                    {
                        //check if value is an Array of objects
                        if (itm.Type.ToString() == "Object")
                        {
                            TreeNode objTN = new TreeNode();
                            //child.Text = token.Key.ToString();
                            //call back the method
                            ix++;

                            JObject o = (JObject)itm;
                            objTN = Json2Tree(o, token.Key + "[" + ix + "]");
                            child.Nodes.Add(objTN);
                            //parent.Nodes.Add(child);
                        }
                        //regular array string, int, etc
                        else if (itm.Type.ToString() == "Array")
                        {
                            ix++;
                            TreeNode dataArray = new TreeNode();
                            foreach (var data in itm)
                            {
                                dataArray.Text = token.Key.ToString() + "[" + ix + "]";
                                dataArray.Nodes.Add(data.ToString());
                            }
                            child.Nodes.Add(dataArray);
                        }

                        else
                        {
                            child.Nodes.Add(itm.ToString());
                        }
                    }
                    parent.Nodes.Add(child);
                }
                else
                {
                    //if token.Value is not nested
                    child.Text = $"{token.Key}: {token.Value.ToString()}";
                    parent.Nodes.Add(child);
                }
            }
            return parent;

        }

        private void eventsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }

        private void eventsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var data = e;

            var selectedItem = this.eventsDataGridView.Rows[e.RowIndex].DataBoundItem as EventModel;

            switch(selectedItem.Event)
            {
                case IMarketEvent m:
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    break;
                case ITaskEvent m:
                    e.CellStyle.ForeColor = Color.Blue;
                    break;
                case IPaymentEvent m:
                    e.CellStyle.ForeColor = Color.Violet;
                    break;
                case ExecutorEvent m:
                    e.CellStyle.ForeColor = Color.Orange;
                    break;
                default:
                    break;
            }

        }

        private void offerModelbindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void agreementModelBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void offersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.offerDetailsTextBox.Text = String.Empty;

            foreach (var row in this.offersDataGridView.SelectedRows)
            {
                var selectedItem = (row as DataGridViewRow).DataBoundItem as OfferModel;

                var text = JsonConvert.SerializeObject(selectedItem.OfferProposal, Formatting.Indented);

                this.offerDetailsTextBox.Text = text;

                this.offerEventBindingSource.DataSource = selectedItem.Events.Select(ev => new EventModel(ev)).ToList();

                break;

            }

        }

        private void offerEventsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.offerEventDetailsTextBox.Text = String.Empty;

            foreach (var row in this.offerEventsDataGridView.SelectedRows)
            {
                var selectedItem = (row as DataGridViewRow).DataBoundItem as EventModel;

                var text = JsonConvert.SerializeObject(selectedItem.Event, Formatting.Indented);

                this.offerEventDetailsTextBox.Text = text;

                break;

            }

        }

        private void offerEventBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void agreementsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            this.agreementDetailsTextBox.Text = String.Empty;

            foreach (var row in this.agreementsDataGridView.SelectedRows)
            {
                var selectedItem = (row as DataGridViewRow).DataBoundItem as AgreementModel;

                var text = JsonConvert.SerializeObject(selectedItem.Agreement, Formatting.Indented);

                this.agreementDetailsTextBox.Text = text;

            }

        }
    }
}
