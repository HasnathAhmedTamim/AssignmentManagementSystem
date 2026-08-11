"use client";

import { useCallback, useEffect, useState } from "react";
import { getErrorMessage } from "@/shared/lib/errors";

interface UseAsyncListOptions<T> {
  loader: () => Promise<T>;
  immediate?: boolean;
}

export function useAsyncList<T>({ loader, immediate = true }: UseAsyncListOptions<T>) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(immediate);
  const [error, setError] = useState("");

  const reload = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setData(await loader());
    } catch (err) {
      setError(getErrorMessage(err, "Failed to load data"));
    } finally {
      setLoading(false);
    }
  }, [loader]);

  useEffect(() => {
    if (immediate) void reload();
  }, [immediate, reload]);

  return {
    data,
    setData,
    loading,
    error,
    setError,
    reload,
  };
}
