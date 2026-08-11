export type FieldRules = Record<string, { value: string; message: string; test?: (value: string) => boolean }>;

/** Returns field errors for empty/invalid values. Empty object means valid. */
export function validateFields(rules: FieldRules): Record<string, string> {
  const errors: Record<string, string> = {};

  for (const [key, rule] of Object.entries(rules)) {
    const value = rule.value.trim();
    const ok = rule.test ? rule.test(rule.value) : Boolean(value);
    if (!ok) errors[key] = rule.message;
  }

  return errors;
}

export function isValidEmail(email: string): boolean {
  return /^\S+@\S+\.\S+$/.test(email.trim());
}
