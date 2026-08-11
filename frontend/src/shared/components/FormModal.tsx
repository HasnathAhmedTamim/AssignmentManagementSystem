"use client";

import { ReactNode } from "react";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";

interface FormModalProps {
  open: boolean;
  title: string;
  formId: string;
  saving?: boolean;
  submitLabel?: string;
  onClose: () => void;
  children: ReactNode;
}

export function FormModal({
  open,
  title,
  formId,
  saving = false,
  submitLabel = "Save",
  onClose,
  children,
}: FormModalProps) {
  return (
    <Modal
      open={open}
      title={title}
      onClose={onClose}
      footer={
        <>
          <Button variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button form={formId} type="submit" loading={saving}>
            {submitLabel}
          </Button>
        </>
      }
    >
      {children}
    </Modal>
  );
}
