# RB.AuthorisationHold.ClientSample

Dæmi um hvernig á að tengjast RB.AuthorisationHold.Api þjónustu.

## Um projecið

Forritað í C# .NET 5.0, þróað í Visual Studio 2019.

## Uppbygging kóðans

**Program.cs**
Sýnir handahófskennt externalId og amount ásamt nýju JWT fyrir viðkomandi umhverfi.

**Core**
  *AccessControl.cs* hjúpar JWT aðgangsstýringu.
  *AuthorisationHold.cs* BLL lag fyrir heimildarþjónustan (hjúpar og kallar).

**Entities**
  *AccessToken.cs* geymir upplýsingar um auðkenninguna.
  *BadRequestProblemDetails.cs* gagnagrind fyrir villur sem koma upp (ekki 2xx serían).
  *BaseReservationResponse.cs* gagnagrind fyrir eðlilegt svar.
  *InternalTypes.cs* upptalning á stillingum sem hægt er að nota.
  **ClearingOutput**
    *PaymentReservation.cs* Gagnagrind fyrir frátekt, grunnklasi fyrir offline færslur.
	*MerchantInfo.cs* Upplýsingar um söluaðila.
	*TransferSettlement.cs* Gagnagrind fyrir offline færslur, erfir PaymentReservation.
	*TransferOther.cs* Gagnagrind fyrir annars konar offline færslur, erfir TransferSettlement.

**Interfaces**
  *ICreditTransfers* innborganir.
  *IPing* er þjónustan uppi?
  *IReservations* frátektir.
  *ISettlements* uppgjörshluti.

**Tests**
  *AuthorisationHoldTests.cs* samþættingarpróf fyrir heimildarþjónustu.  

**Utils**
  *Logger* dot4net víðvær hjúpur, býr til ClientSample.log í project folder.
  *StringUtility* hjúpur utan um http samskipti út á við.
  *HostDetailsFactory* skilgreinir tengiupplýsingar fyrir rest þjónustu og skilar af sér.
  - Hér þarf að breyta í samræmi við aðgang.

**Data**
- BuildJsonFile.json og  BuildJsonFileNULL.json
  Búin til með TestBuildJsonFile fallinu í AuhorisationTests.cs.
  NULL skráin er til að sýna fram á öll svæðin í .json skránni.
- Bull.bin
  Textaskrá sem á ekki að vera senda yfir vírinn.
- OfflineSettlements.json
  Dæmi um hvernig uppgjörsskrá getur litið út.
  1336 TransferSettlements og 2 TransferOthers eru í skránni (úr DEV).
- OfflineSettlements.zip
  Zippuð ofangreind skrá, svona viljum við fá uppgjörið.