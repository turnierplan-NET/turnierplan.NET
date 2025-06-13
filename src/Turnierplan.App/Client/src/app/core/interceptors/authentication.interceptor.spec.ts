import { AuthenticationInterceptor } from './authentication.interceptor';

describe('AuthenticationInterceptor', () => {
  it('should determine correctly whether authentication is required or not', () => {
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/api`)).toBeTrue();
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/api/test`)).toBeTrue();
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/api/test/identity`)).toBeTrue();
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/api/identity`)).toBeFalse();
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/api/identity/test`)).toBeFalse();
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/something/api`)).toBeFalse();
    expect(AuthenticationInterceptor.doesPathRequireAuthentication(`/something/api/identity`)).toBeFalse();
  });
});
