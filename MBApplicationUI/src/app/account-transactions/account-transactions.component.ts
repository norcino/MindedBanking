import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.css']
})
export class AccountTransactionsComponent implements OnInit {
  loading: boolean;

  constructor() {
    this.loading = true;
  }

  ngOnInit(): void {
  }

}
