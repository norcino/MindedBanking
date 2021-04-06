import { Currency } from "./currency";

export class Transaction {
  public id?: number;
  public description: string;
  public amount?: number;
  public originalAmount: number;
  public currencyId: number;
  public currency?: Currency;
  public accountId: number;
  public dateTime?: string;

  constructor(description: string, originalAmount: number, currencyId: number, accountId: number){
    this.description = description;
    this.originalAmount = originalAmount;
    this.currencyId = currencyId;
    this.accountId = accountId;
  }
}
