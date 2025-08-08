import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_service/account.service';
import { inject } from '@angular/core';

export const AuthGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);

  if (accountService.currentUser()) {
    return true;
  } else {
    return false;
  }
};