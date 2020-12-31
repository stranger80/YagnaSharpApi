

## ToDo list

- Implement cancellation token in "collect" methods with long timeouts
  - Api clients
  - Repository classes
- Implement Subscription Entity CollectOffers() 
- Implement the proposal entity Respond() and Reject()
- Implement AgreementPool 
  - including also the Agreement negotiation/confirmation
- Implement ActivityRepository
  - Including the Command Result events processing!
- Implement GFTP StorageProvider
- Implement WorkerStarter() - fetch a confirmed agreement from the pool and run a worker on this agreement
- Implement StartWorker() logic - for a given Agreement, start activity and execute an exescript against it, then accept the payment
- Implement GetInvoiceEventAsync() to continuously listen on the invoiceEvents endpoint (remember to use the afterTimestamp parameter!)
- Implement ProcessInvoicesAsync() logic
- Implement AcceptPaymentForAgreement() logic

- Implement the Executor event notification framework


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
