import { doesPathRequireAuthentication } from './authentication.interceptor';

describe('AuthenticationInterceptor', () => {
  it('should determine correctly whether authentication is required or not', () => {
    expect(doesPathRequireAuthentication(`/api`)).toBeTrue();
    expect(doesPathRequireAuthentication(`/api/test`)).toBeTrue();
    expect(doesPathRequireAuthentication(`/api/test/identity`)).toBeTrue();
    expect(doesPathRequireAuthentication(`/auth`)).toBeFalse();
    expect(doesPathRequireAuthentication(`/auth/login`)).toBeFalse();
    expect(doesPathRequireAuthentication(`/something/api`)).toBeFalse();
    expect(doesPathRequireAuthentication(`/something/api/identity`)).toBeFalse();
  });
});
