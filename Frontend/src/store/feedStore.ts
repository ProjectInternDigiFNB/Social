import { create } from 'zustand';
import type { PostDto } from '@/types';

export type FeedTab = 'for-you' | 'following' | 'recent';

interface FeedTabState {
  posts: PostDto[];
  page: number;
  hasMore: boolean;
  scrollY: number;
}

const initialTabState = (): FeedTabState => ({
  posts: [],
  page: 1,
  hasMore: true,
  scrollY: 0,
});

interface FeedStoreState {
  activeTab: FeedTab;
  /**
   * Normalized State (Single Source of Truth):
   * Lưu trữ thực thể bài viết dạng Dictionary để truy cập O(1) và tránh phân mảnh trạng thái giữa các tab
   */
  entities: Record<string, PostDto>;
  tabStates: Record<FeedTab, FeedTabState>;

  setActiveTab: (tab: FeedTab) => void;
  setTabPosts: (tab: FeedTab, posts: PostDto[], page: number, hasMore: boolean) => void;
  appendTabPosts: (tab: FeedTab, newPosts: PostDto[], page: number, hasMore: boolean) => void;
  prependTabPost: (post: PostDto) => void;
  removeTabPost: (postId: string) => void;
  updatePostInFeed: (postId: string, updater: (p: PostDto) => PostDto) => void;
  setTabScrollY: (tab: FeedTab, scrollY: number) => void;
  clearTab: (tab: FeedTab) => void;
  getPostById: (postId: string) => PostDto | undefined;
}

export const useFeedStore = create<FeedStoreState>((set, get) => ({
  activeTab: 'for-you',
  entities: {},
  tabStates: {
    'for-you': initialTabState(),
    following: initialTabState(),
    recent: initialTabState(),
  },

  setActiveTab: (tab) => set({ activeTab: tab }),

  getPostById: (postId) => {
    return get().entities[postId.toLowerCase()];
  },

  setTabPosts: (tab, posts, page, hasMore) =>
    set((state) => {
      const nextEntities = { ...state.entities };
      for (const p of posts) {
        nextEntities[p.id.toLowerCase()] = p;
      }

      return {
        entities: nextEntities,
        tabStates: {
          ...state.tabStates,
          [tab]: {
            ...state.tabStates[tab],
            posts,
            page,
            hasMore,
          },
        },
      };
    }),

  appendTabPosts: (tab, incoming, page, hasMore) =>
    set((state) => {
      const current = state.tabStates[tab].posts;
      if (incoming.length === 0) {
        return {
          tabStates: {
            ...state.tabStates,
            [tab]: {
              ...state.tabStates[tab],
              page,
              hasMore,
            },
          },
        };
      }

      // Tránh lặp ngay lập tức các bài viết vừa hiển thị ở cuối feed
      const recentWindow = current.slice(-Math.max(incoming.length, 5));
      const recentIds = new Set(recentWindow.map((p) => p.id));
      let toAppend = incoming.filter((p) => !recentIds.has(p.id));

      // Khi đã lướt hết toàn bộ bài viết mới và hệ thống tuần hoàn lại (hoặc số bài ít),
      // chấp nhận nạp tiếp incoming để feed tiếp tục cuộn vô hạn không ngắt quãng
      if (toAppend.length === 0) {
        toAppend = incoming;
      }

      const nextEntities = { ...state.entities };
      for (const p of toAppend) {
        nextEntities[p.id.toLowerCase()] = p;
      }

      return {
        entities: nextEntities,
        tabStates: {
          ...state.tabStates,
          [tab]: {
            ...state.tabStates[tab],
            posts: [...current, ...toAppend],
            page,
            hasMore,
          },
        },
      };
    }),

  prependTabPost: (newPost) =>
    set((state) => {
      const targetId = newPost.id.toLowerCase();
      const nextEntities = {
        ...state.entities,
        [targetId]: newPost,
      };

      const updated = { ...state.tabStates };
      for (const tab of ['for-you', 'following', 'recent'] as FeedTab[]) {
        const current = updated[tab].posts;
        if (!current.some((p) => p.id === newPost.id)) {
          updated[tab] = {
            ...updated[tab],
            posts: [newPost, ...current],
          };
        }
      }
      return {
        entities: nextEntities,
        tabStates: updated,
      };
    }),

  removeTabPost: (postId) =>
    set((state) => {
      const targetId = postId.toLowerCase();
      const nextEntities = { ...state.entities };
      delete nextEntities[targetId];

      const updated = { ...state.tabStates };
      for (const tab of ['for-you', 'following', 'recent'] as FeedTab[]) {
        updated[tab] = {
          ...updated[tab],
          posts: updated[tab].posts.filter((p) => p.id.toLowerCase() !== targetId),
        };
      }
      return {
        entities: nextEntities,
        tabStates: updated,
      };
    }),

  updatePostInFeed: (postId, updater) =>
    set((state) => {
      const targetId = postId.toLowerCase();
      const existing = state.entities[targetId];
      const updatedPost = existing ? updater(existing) : null;

      const nextEntities = { ...state.entities };
      if (updatedPost) {
        nextEntities[targetId] = updatedPost;
      }

      const updated = { ...state.tabStates };
      for (const tab of ['for-you', 'following', 'recent'] as FeedTab[]) {
        updated[tab] = {
          ...updated[tab],
          posts: updated[tab].posts.map((p) => {
            if (p.id.toLowerCase() === targetId) {
              return updatedPost || updater(p);
            }
            return p;
          }),
        };
      }
      return {
        entities: nextEntities,
        tabStates: updated,
      };
    }),

  setTabScrollY: (tab, scrollY) =>
    set((state) => ({
      tabStates: {
        ...state.tabStates,
        [tab]: {
          ...state.tabStates[tab],
          scrollY,
        },
      },
    })),

  clearTab: (tab) =>
    set((state) => ({
      tabStates: {
        ...state.tabStates,
        [tab]: initialTabState(),
      },
    })),
}));
