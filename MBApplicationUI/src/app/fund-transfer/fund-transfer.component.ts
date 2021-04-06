import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgForm, NgModel } from '@angular/forms';
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
  @Input()
  public accountId?: number = 0;
  @Output() notify: EventEmitter<boolean> = new EventEmitter();
  confirm: string = "";
  currencies: Currency[] = [];
  message: string = "";
  loading: boolean = true;
  completed: boolean = false;
  isErrorNotification: boolean = false;

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    this.confirm = this.deposit ? "Deposit" :  "Withdraw";

    if(!this.accountId) {
      this.message = "Account not valid";
      this.isErrorNotification = true;
      this.loading = false;
    }

    this.apiService.getSupportedCurrencies().subscribe({
      next: result => {
        this.currencies = result;
        this.loading = false;
      },
      error: message => {
        this.message = message;
        this.completed = true;
        this.isErrorNotification = true;
        this.loading = false;
      }
    });
  }

  closeFunds() {
    this.notify.emit(true);
  }

  validateAmount(field: NgModel) {
    if(field.value < 0.01) {
      field.valueAccessor?.writeValue(null);
      field.control.setErrors({'invalid': true});
    }
  }

  onSubmit(form: NgForm) {
    var transactionAmount = form.value.amount;
    if(!this.deposit) transactionAmount = form.value.amount * -1;

    let transaction = new Transaction(form.value.description, transactionAmount, form.value.currencies, Number(this.accountId));

    this.apiService.postTransaction(transaction).subscribe({
      next: result => {
        this.message = "Transaction completed successfully"
        this.completed = true;
      },
      error: message => {
        this.message = message.error;
        this.isErrorNotification = true;
        this.completed = true;
      }
    });
  }
}
