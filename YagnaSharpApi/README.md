

## ToDo list

- (DONE) Implement cancellation token in "collect" methods with long timeouts
  - Api clients
  - Repository classes
- (DONE) Implement Subscription Entity CollectOffers() 
- (DONE) Implement the proposal entity Respond() and Reject()
- (DONE) Refactor the offer finding code to be part of MarketStrategy
- Implement DummyMarketStrategy
  - Add MarketStrategyConditions - supported payment platforms
  - (IN PROGRESS) Write unit tests for MarketStrategyBase
- Implement AgreementPool 
  - including also the Agreement negotiation/confirmation
  - consider generating events - should AgreementPool raise agreement-related events?
- Finish implementing AgreementEntity
  - (PARTIALLY DONE) implement AgreementEntity state machine (use state pattern, remember locking/semaphores)
  - how to observe AgreementEvents
    - (PARTIALLY DONE) Implement OnAgreementEvent hook in MarketRepository and hook all Agreements into this to observe state changing events
    - (DONE needs testing) in MarketRepository launch a thread to continuously listen to agreement events and dispatch (WITH CANCELLATION TOKEN! )
- Implement ActivityRepository
  - Including the Command Result events processing!
- (DONE) Implement GFTP StorageProvider
- Implement WorkerStarter() - fetch a confirmed agreement from the pool and run a worker on this agreement
- Implement StartWorker() logic - for a given Agreement, start activity and execute an exescript against it, then accept the payment
- Implement GetInvoiceEventAsync() to continuously listen on the invoiceEvents endpoint (remember to use the afterTimestamp parameter!)
  - actually implement this in PaymentRepository as OnInvoiceEvent handler
- Implement ProcessInvoicesAsync() logic
- Implement AcceptPaymentForAgreement() logic

- Implement the Executor event notification framework
  - Partially done for MarketStrategy - using C# events


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
