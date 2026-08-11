"use client";

import { useCallback, useState } from "react";
import { getErrorMessage } from "@/shared/lib/errors";

interface UseCrudModalOptions {
  onSaved?: () => Promise<void> | void;
}

/** Shared create/edit modal + save/delete helpers for admin CRUD screens. */
export function useCrudModal({ onSaved }: UseCrudModalOptions = {}) {
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const close = useCallback(() => {
    setOpen(false);
    setFieldErrors({});
  }, []);

  const openModal = useCallback(() => {
    setFieldErrors({});
    setError("");
    setOpen(true);
  }, []);

  const runSave = useCallback(
    async (action: () => Promise<unknown>) => {
      setSaving(true);
      setError("");
      try {
        await action();
        setOpen(false);
        await onSaved?.();
      } catch (err) {
        setError(getErrorMessage(err, "Save failed"));
      } finally {
        setSaving(false);
      }
    },
    [onSaved]
  );

  const runDelete = useCallback(
    async (message: string, action: () => Promise<void>) => {
      if (!confirm(message)) return;
      setError("");
      try {
        await action();
        await onSaved?.();
      } catch (err) {
        setError(getErrorMessage(err, "Delete failed"));
      }
    },
    [onSaved]
  );

  return {
    open,
    setOpen,
    saving,
    error,
    setError,
    fieldErrors,
    setFieldErrors,
    openModal,
    close,
    runSave,
    runDelete,
  };
}
