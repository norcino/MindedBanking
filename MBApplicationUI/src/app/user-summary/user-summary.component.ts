import { Component, Input, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { User } from '../model/user';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-summary',
  templateUrl: './user-summary.component.html',
  styleUrls: ['./user-summary.component.css']
})
export class UserSummaryComponent implements OnInit {
  user: User = new User(0,'','');
  loading: boolean = true;
  errorMessage: string = "";
  showFundsTransfer: boolean = false;
  isDepositTransfer: boolean = true;

  constructor(private apiService: ApiService, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    var id = this.activatedRoute.snapshot.paramMap.get('id');
    this.apiService.getUserById(Number(id)).subscribe({
      next: result => {
        this.user = result;
        this.loading = false;
      },
      error: message => {
        console.log("Receved error: " + message);
        this.errorMessage = message;
        this.loading = false;
      }
    });
  }

  withdraw() {
    this.isDepositTransfer = false;
    this.showFundsTransfer = true;
  }

  deposit(){
    this.isDepositTransfer = true;
    this.showFundsTransfer = true;
  }

  closeFunds(status: boolean) {
    this.showFundsTransfer = !status;
  }
}
