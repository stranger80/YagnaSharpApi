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
