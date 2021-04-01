import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Transaction } from './model/transaction';
import { Account } from './model/account';
import { Currency } from './model/currency';
import { User } from './model/user';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  baseURL: string = "https://localhost:5001/api";

  constructor(private http: HttpClient) {
  }

  getUserById(userId: number): Observable<User> {
    return this.http.get<User>(this.baseURL + '/Users/' + userId);
  }

  getTransactionsByAccountId(accountId: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(this.baseURL + '/Transactions/' + accountId);
  }

  getAccountByUserId(userId: number): Observable<Account[]> {
    return this.http.get<Account[]>(this.baseURL + '/Accounts?filter=UserId eq ' + userId + '&$expand=DefaultCurrency');
  }

  getSupportedCurrencies(): Observable<Currency[]> {
    return this.http.get<Currency[]>(this.baseURL + '/Currencies');
  }

  getAccountBalanceByAccountId(accountId: number): Observable<number> {
    return this.http.get<number>(this.baseURL + '/Accounts/' + accountId + '/balance');
  }

  postTransaction(transaction: Transaction): Observable<Transaction> {
    return this.http.post<Transaction>(this.baseURL + '/Transactions', transaction);
  }
}
