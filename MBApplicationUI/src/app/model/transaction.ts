export class Transaction {
  public id: number;
  public description: string;
  public amount: number;
  public originalAmount: number;
  public currencyId: number;
  public accountId: number;
  public dateTime: string;

  constructor(id: number,
    description: string,
    amount: number,
    originalAmount: number,
    currencyId: number,
    accountId: number,
    dateTime: string){
    this.id = id;
    this.description = description;
    this.amount = amount;
    this.originalAmount = originalAmount;
    this.currencyId = currencyId;
    this.accountId = accountId;
    this.dateTime = dateTime;
  }
}
