import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AccountSummaryComponent } from './account-summary/account-summary.component';
import { UserSummaryComponent } from './user-summary/user-summary.component';
import { FundTransferComponent } from './fund-transfer/fund-transfer.component';
import { AccountTransactionsComponent } from './account-transactions/account-transactions.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SpinnerComponent } from './spinner/spinner.component';
import { ApiService } from './api.service';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'account/:id', component: AccountSummaryComponent },
  { path: 'account', redirectTo: '/home', pathMatch: 'full' },
  { path: 'user/:id', component: UserSummaryComponent },
  { path: 'user', redirectTo: '/home', pathMatch: 'full' },
  { path: '', component: AppComponent },
  { path: '**', component: AppComponent }
]

@NgModule({
  declarations: [
    AppComponent,
    AccountSummaryComponent,
    UserSummaryComponent,
    FundTransferComponent,
    AccountTransactionsComponent,
    SpinnerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule.forRoot(routes)
  ],
  providers: [ApiService, HttpClientModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
