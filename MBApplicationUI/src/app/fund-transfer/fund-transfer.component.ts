import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ApiService } from '../api.service';
import { Currency } from '../model/currency';
import { Transaction } from '../model/transaction';

@Component({
  selector: 'app-fund-transfer',
  templateUrl: './fund-transfer.component.html',
  styleUrls: ['./fund-transfer.component.css']
})
export class FundTransferComponent implements OnInit {
  @Input()
  public deposit: boolean = true;
  @Output() notify: EventEmitter<boolean> = new EventEmitter();
  description: string = "";
  amount: number = 0;
  currency: string = "";
  confirm: string = "";
  currencies: Currency[] = [];
  errorMessage: string = "";
  loading: boolean = true;

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.confirm = this.deposit ? "Deposit" :  "withdraw";

    this.apiService.getSupportedCurrencies().subscribe({
      next: result => {
        this.currencies = result;
        this.loading = false;
      },
      error: message => {
        console.log("Receved error: " + message);
        this.errorMessage = message;
        this.loading = false;
      }
    });
  }

  closeFunds() {
    this.notify.emit(true);
  }

  transferFunds(){
    var currencyID = 1;
    var accountID = 1;
    var transaction = new Transaction(0,this.description, this.amount, 0, currencyID, accountID, '');

    this.apiService.postTransaction(transaction).subscribe({
      next: result => {
        console.log(result);
      },
      error: message => {
        console.log("Receved error: " + message);
        this.errorMessage = message;
      }
    });
  }

  changedCurrency(e: Event) {

  }
}
