import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { Account } from '../model/account';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-account-summary',
  templateUrl: './account-summary.component.html',
  styleUrls: ['./account-summary.component.css']
})
export class AccountSummaryComponent implements OnInit {
  account: Account = new Account(0,0,0,0);
  loading: boolean = true;
  errorMessage: string = "";
  balance: number = 0;
  constructor(private apiService: ApiService, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      let userId = +params['uid'];
      if(!userId || userId == 0)
        return;

      this.apiService.getAccountByUserId(userId).subscribe({
        next: result => {
          this.account = result[0];

          this.apiService.getAccountBalanceByAccountId(this.account.id).subscribe({
            next: (balance: number) => {
              this.balance = balance;
              this.loading = false;
            },
            error: (message: string) => {
              this.handleError(message);
            }
          });
        },
        error: message => {
          this.handleError(message);
        }
      });
    });
  }
  handleError(message: string) {
    console.log("Receved error: " + message);
    this.errorMessage = message;
    this.loading = false;
  }
}
