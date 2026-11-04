export interface JwtPayload {
  sub?: string;
  email?: string;
  role?: string;
  exp?: number;
  iat?: number;
  [key: string]: unknown;
}

/**
 * Giải mã phần payload của chuỗi JWT một cách an toàn (hỗ trợ Base64Url và UTF-8)
 */
export function decodeTokenPayload<T = JwtPayload>(token: string | null): T | null {
  if (!token) return null;
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;

    let base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    while (base64.length % 4) {
      base64 += '=';
    }

    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );

    return JSON.parse(jsonPayload) as T;
  } catch {
    return null;
  }
}

/**
 * Kiểm tra xem token JWT có hợp lệ và còn hạn sử dụng hay không
 * @param token Chuỗi Access Token JWT
 * @param bufferSeconds Thời gian trừ hao an toàn trước khi hết hạn (mặc định 30 giây)
 */
export function isTokenValid(token: string | null, bufferSeconds = 30): boolean {
  if (!token) return false;
  const payload = decodeTokenPayload(token);
  if (!payload || typeof payload.exp !== 'number') return false;

  const nowInSeconds = Math.floor(Date.now() / 1000);
  return payload.exp > nowInSeconds + bufferSeconds;
}
