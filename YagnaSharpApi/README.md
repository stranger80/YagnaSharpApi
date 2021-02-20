

## ToDo list

- (DONE) Implement cancellation token in "collect" methods with long timeouts
  - Api clients
  - Repository classes
- (DONE) Implement Subscription Entity CollectOffers() 
- (DONE) Implement the proposal entity Respond() and Reject()
- (DONE) Refactor the offer finding code to be part of MarketStrategy
- Implement DummyMarketStrategy
  - (DONE) Add MarketStrategySettings - supported payment platforms
  - (DONE) Write unit tests for MarketStrategyBase
- Implement AgreementPool 
  - (DONE) including also the Agreement negotiation/confirmation
  - (DONE) consider generating events - should AgreementPool raise agreement-related events?
- Finish implementing AgreementEntity
  - (PARTIALLY DONE) implement AgreementEntity state machine (use state pattern, remember locking/semaphores)
  - how to observe AgreementEvents
    - (PARTIALLY DONE) Implement OnAgreementEvent hook in MarketRepository and hook all Agreements into this to observe state changing events
    - (DONE needs testing) in MarketRepository launch a thread to continuously listen to agreement events and dispatch (WITH CANCELLATION TOKEN! )
- (DONE) Implement GFTP StorageProvider
- (PARTIALLY DONE)Implement ActivityRepository
  - (DONE) Basic API commands
  - (DONE)ActivityEntity
  - Including the Command Result events processing!
- (PARTIALLY DONE) Implement WorkerStarter() - fetch a confirmed agreement from the pool and run a worker on this agreement
- (PARTIALLY DONE) Implement StartWorker() logic - for a given Agreement, start activity and execute an exescript against it, then accept the payment
- (DONE) Implement GetInvoiceEventAsync() to continuously listen on the invoiceEvents endpoint (remember to use the afterTimestamp parameter!)
- Implement ProcessInvoicesAsync() logic
  - (DONE) add "agreementsPayable" list to Executor
  - (DONE) when invoice arrives, check if agreement in payable list
  - (DONE) Implement AcceptPaymentForAgreement() logic
- TODO Implement ProcessDebitNotes() logic
  - todo as in Invoices...

- TODO Add to Executor.Dispose() removal of all allocations made...

- (PARTIALLY DONE) Implement GolemTask logic
  - Add GolemTask.OnTaskComplete(sender, status) event handler and call the OnTaskComplete from GolemTask.Accept() and Reject()
  - instead "var commandGenerator = worker(workContext, AsyncEnumerable.ToAsyncEnumerable(data));" 
    - make an async iterator that calls GolemTask.Start() (and maybe something else to hook to "work queue???")
  - ...remaining logic as in python...

- Implement InvoiceEntity.AcceptAsync()

- Implement Submit() completion logic
  - (DONE) Add 'doneQueue' - and add OnTaskComplete observer to add completed GolemTask to doneQueue.
  - (DONE) Handle task retry in case COmmandResult error or other error is received by the Worker 
  - (PARTIALLY DONE) wait for all invoices to be paid
  - HOW TO make the invoices come faster? TerminateAgreement()?

- TODO troubleshooting
  - sometimes agreement can be null in AgreementPool.cs line 155... 
  - Newtonsoft error on deserialize when Subscribing Demand - nondeterministic...

- (PARTIALLY DONE) Implement the Executor event notification framework
  - Partially done for MarketStrategy - using C# events
  - TODO Add ComputationFinished()


## Component notes:

DemandBuilder
  - Construct the properties (incl. task_package) and constraints

SubscriptionEntity
- this should probably implement the eventlistener (ie. CollectOffer)
- how do we implement reading the Proposals? 
  - Via OnProposal() event handler? 
  - Via in-stream that reads incoming proposals? async in-stream? 
  - Via a blocking queue?

AgreementEntity
  - implement HandleAgreementEvent() to react to respective events
  - provide OnEvent() handler to hook into incoming Agreement-related events

AgreementRepository
  - store the AgreementEntities so that they can be retrieved by AgreementId by eg. AgreementListener.
  - ensure there is one AgreementId object for a AgreementId

AgreementListener
  - handle the collectAgreementEvents
  - dispatch the incoming agreement events to relevant AgreementEntities

ActivityEntity
  - Exec(ExeScript)
  - Destroy()

InvoiceEntity
  - HandleEvent() to process incoming InvoiceEvents
  - reference to AgreementEntity?
  - Accept() and Reject()

InvoiceRepository
  - store of Invoices by InvoiceId

InvoiceListener
  - 

DebitNoteEntity
DebitNoteRepository
DebitNoteListener
...same as for Invoices

Task
  - description of the work to execute
  - task package
  - input files to send
  - command to run
  - output files to collect


Engine


MarketStrategy
- "decorate demand" - not sure yet what the purpose would be
- OfferScore(OfferProposal) - to return scoring of a given offer - so that the Engine can order the Offers
