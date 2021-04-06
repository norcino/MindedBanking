import { Component, Input, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { CsvExportService } from '../csv-export.service';
import { Transaction } from '../model/transaction';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.css']
})
export class AccountTransactionsComponent implements OnInit {
  @Input()
  public accountId: number = 0;
  @Input()
  public defaultCurrency: string = "";
  transactions: Transaction[] = [];
  loading: boolean;
  errorMessage: string = "";

  constructor(private apiService: ApiService, private csvExportService: CsvExportService) {
    this.loading = true;
  }

  ngOnInit(): void {
    this.apiService.getTransactionsByAccountId(this.accountId).subscribe({
      next: result => {
        console.log(result);
        this.transactions = result;
        this.loading = false;
      },
      error: message => {
        this.handleError(message);
      }
    });
  }

  export() {
    this.csvExportService.downloadFile(this.transactions, ['description','amount','originalAmount','currency.code','dateTime'], ['Description','Amount','Original Amount','Currency','Date'])
  }

  handleError(message: string) {
    console.log("Receved error: " + message);
    this.errorMessage = message;
    this.loading = false;
  }
}
