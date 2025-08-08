import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_service/account.service';

export const AdminGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  if (accountService.role().includes('Admin')) return true;
  return false;
};