import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_service/account.service';
import { User } from '../_model/user';

@Directive({
    selector: '[appHasRole]',
    standalone: true
})
export class HasRoleDirective implements OnInit{
  private accountService = inject(AccountService);
  @Input() appHasRole: string[] = [];

  constructor(private viewContainerRef: ViewContainerRef, 
              private templateRef: TemplateRef<any>) {}
  ngOnInit(): void {
    const user = this.accountService.currentUser();
    if (!user || !user.token) {
      this.viewContainerRef.clear();
      return;
    }
    try {
      const userRole = JSON.parse(atob(user.token.split('.')[1]))?.role;
      if (this.appHasRole.includes(userRole)) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear();
      }
    } catch (e) {
      console.error('Error decoding token for role:', e);
      this.viewContainerRef.clear();
    }
  }
}