import { Currency } from "./currency";

export class Account {
  public id: number;
  public number: number;
  public defaultCurrencyId: number;
  public defaultCurrency: Currency;
  public userId: number;

  constructor(id: number, number: number, defaultCurrencyId: number, userId: number){
    this.id = id;
    this.number = number;
    this.defaultCurrencyId = defaultCurrencyId;
    this.userId = userId;
    this.defaultCurrency = new Currency(0,'','');
  }
}
