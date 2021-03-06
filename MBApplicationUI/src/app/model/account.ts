import { Currency } from "./currency";

export class Account {
  public id?: number;
  public number: number;
  public defaultCurrencyId: number;
  public defaultCurrency: Currency;
  public userId: number;

  constructor(number: number, defaultCurrencyId: number, userId: number){
    this.number = number;
    this.defaultCurrencyId = defaultCurrencyId;
    this.userId = userId;
    this.defaultCurrency = new Currency(0,'','');
  }
}
