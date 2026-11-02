/**
 * Helper utility để tự động tối ưu hóa URL hình ảnh và media (Cloudinary / CDN)
 * Áp dụng các cờ tối ưu hóa tiêu chuẩn:
 * - f_auto: Tự động chuyển đổi định dạng tối ưu nhất theo trình duyệt (AVIF, WebP)
 * - q_auto: Tự động nén chất lượng thông minh mà mắt thường không phân biệt được
 * - w_{width}: Thu nhỏ ảnh theo đúng kích thước khung hiển thị trên thiết bị
 */

interface OptimizeImageOptions {
  width?: number;
  height?: number;
  quality?: 'auto' | 'auto:good' | 'auto:best' | 'auto:eco' | 'auto:low';
  format?: 'auto' | 'webp' | 'avif' | 'jpg' | 'png';
  crop?: 'limit' | 'fill' | 'thumb' | 'fit';
}

export function getOptimizedImageUrl(
  url?: string | null,
  options: OptimizeImageOptions = {}
): string {
  if (!url || typeof url !== 'string') return '';

  // Kiểm tra xem có phải ảnh từ Cloudinary hay không
  if (url.includes('res.cloudinary.com') && url.includes('/upload/')) {
    const {
      width,
      height,
      quality = 'auto',
      format = 'auto',
      crop = 'limit',
    } = options;

    const transformations: string[] = [`f_${format}`, `q_${quality}`];

    if (width) transformations.push(`w_${width}`);
    if (height) transformations.push(`h_${height}`);
    if (width || height) transformations.push(`c_${crop}`);

    const transformStr = transformations.join(',');

    // Tránh chèn trùng nếu URL đã có transformation
    if (url.includes(`/upload/${transformStr}/`)) {
      return url;
    }

    return url.replace('/upload/', `/upload/${transformStr}/`);
  }

  // URL thông thường hoặc ảnh cục bộ
  return url;
}

