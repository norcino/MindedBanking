import { Account } from "./account";
export class User {
  public id: number;
  public name: string;
  public surname: string;
  public account?: Account;

  constructor(id: number, name : string, surname : string){
    this.id = id;
    this.name = name;
    this.surname = surname;
  }
}
