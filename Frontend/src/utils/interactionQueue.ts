import { interactionApi } from '@/api/interactionApi';
import type { TrackInteractionRequestDto } from '@/types';

/**
 * Hàng đợi đệm (Telemetry Batching Queue - tương đương unified_cvc của Facebook)
 * Thay vì gửi 1 HTTP request cho từng bài viết lướt qua, hàng đợi gom lại và
 * gửi 1 mẻ duy nhất sau mỗi 2.5s hoặc khi tích lũy đủ 10 sự kiện.
 */

const FLUSH_INTERVAL_MS = 2500;
const BATCH_SIZE_THRESHOLD = 10;

let queue: TrackInteractionRequestDto[] = [];
let flushTimer: ReturnType<typeof setTimeout> | null = null;
const recentViewedPostIds = new Set<string>();

function flushQueue(): void {
  if (flushTimer) {
    clearTimeout(flushTimer);
    flushTimer = null;
  }

  if (queue.length === 0) return;

  const eventsToFlush = [...queue];
  queue = [];

  // Gửi mẻ lên backend ngầm
  interactionApi.trackBatch({ events: eventsToFlush }).catch(() => {
    // Background tracking silently catches errors
  });
}

// Tự động xả hàng đợi khi người dùng rời tab hoặc đóng trình duyệt
if (typeof window !== 'undefined') {
  window.addEventListener('beforeunload', () => {
    flushQueue();
  });

  document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'hidden') {
      flushQueue();
    }
  });
}

export const interactionQueue = {
  /**
   * Đưa một sự kiện tương tác vào hàng đợi đệm
   */
  enqueue(event: TrackInteractionRequestDto): void {
    if (!event.postId && !event.metadata) return;

    // Khử trùng lặp cho sự kiện View trong cùng 1 phiên lướt gần nhất
    if (event.postId && event.interactionType === 1) {
      if (recentViewedPostIds.has(event.postId)) {
        return;
      }
      recentViewedPostIds.add(event.postId);
      // Giới hạn bộ nhớ cache khử trùng lặp
      if (recentViewedPostIds.size > 200) {
        const firstEntry = recentViewedPostIds.values().next().value;
        if (firstEntry) recentViewedPostIds.delete(firstEntry);
      }
    }

    queue.push(event);

    if (queue.length >= BATCH_SIZE_THRESHOLD) {
      flushQueue();
    } else if (!flushTimer) {
      flushTimer = setTimeout(flushQueue, FLUSH_INTERVAL_MS);
    }
  },

  /**
   * Ép xả ngay lập tức toàn bộ sự kiện đang chờ
   */
  flush(): void {
    flushQueue();
  },
};

